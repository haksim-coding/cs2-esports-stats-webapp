using cs2_esports.Models;
using cs2_esports.Helpers;
using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace cs2_esports.Controllers;

public class PlayersController : Controller
{
    private static readonly byte[] PngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
    private static readonly byte[] RiffSignature = { 0x52, 0x49, 0x46, 0x46 };
    private static readonly byte[] WebpSignature = { 0x57, 0x45, 0x42, 0x50 };
    private const long MaxPlayerImageBytes = 4 * 1024 * 1024;
    private readonly IPlayerRepository _playerRepository;
    private readonly IForumRepository _forumRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IWebHostEnvironment _environment;
    private const string ForumUserSessionKey = "ForumUserId";

    public PlayersController(IPlayerRepository playerRepository, IForumRepository forumRepository, ITeamRepository teamRepository, IWebHostEnvironment environment)
    {
        _playerRepository = playerRepository;
        _forumRepository = forumRepository;
        _teamRepository = teamRepository;
        _environment = environment;
    }

    public IActionResult Index()
    {
        var players = _playerRepository.GetAllAlphabetical();
        ApplyFavoriteState(players);
        return View(players);
    }

    public IActionResult Details(int id)
    {
        var player = _playerRepository.GetById(id);
        if (player is null)
        {
            return NotFound();
        }

        ApplyFavoriteState(new[] { player });
        return View(player);
    }

    [HttpGet("/player/{slug}")]
    public IActionResult DetailsBySlug(string slug)
    {
        var playerSummary = _playerRepository.GetAllAlphabetical().FirstOrDefault(player =>
            RouteSlugHelper.MatchesRouteSegment(player.Nickname, slug));

        if (playerSummary is null)
        {
            return NotFound();
        }

        var player = _playerRepository.GetById(playerSummary.Id);
        if (player is null)
        {
            return NotFound();
        }

        ApplyFavoriteState(new[] { player });
        return View("Details", player);
    }

    [HttpGet]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult Create()
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return NotFound();
        }

        var model = new PlayerCreateModel
        {
            DateOfBirth = new DateTime(2000, 1, 1)
        };

        PopulateTeams(model.TeamId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult Create(PlayerCreateModel model)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return NotFound();
        }

        ValidatePlayer(model);
        ValidatePlayerImage(model.PlayerImage, nameof(model.PlayerImage));

        if (!ModelState.IsValid)
        {
            PopulateTeams(model.TeamId);
            return View(model);
        }

        var player = MapToPlayer(model);
        var imageUpload = SavePlayerImage(model.PlayerImage);
        if (imageUpload is not null)
        {
            ApplyImageMetadata(player, imageUpload);
        }
        _playerRepository.Add(player);
        return RedirectToAction(nameof(DetailsBySlug), new { slug = RouteSlugHelper.ToRouteSegment(player.Nickname) });
    }

    [HttpGet]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult Edit(int id)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return NotFound();
        }

        var player = _playerRepository.GetById(id);
        if (player is null)
        {
            return NotFound();
        }

        var model = MapToEditModel(player);
        PopulateTeams(model.TeamId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult Edit(PlayerEditModel model)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return NotFound();
        }

        var player = _playerRepository.GetById(model.Id);
        if (player is null)
        {
            return NotFound();
        }

        ValidatePlayer(model, model.Id);
        ValidatePlayerImage(model.PlayerImage, nameof(model.PlayerImage));

        if (!ModelState.IsValid)
        {
            PopulateTeams(model.TeamId);
            model.CurrentImagePath = player.ImagePath;
            return View(model);
        }

        var previousImagePath = player.ImagePath;
        var imageUpload = SavePlayerImage(model.PlayerImage);
        var newImagePath = imageUpload?.Path;

        player.Nickname = model.Nickname.Trim();
        player.FullName = model.FullName.Trim();
        player.CountryCode = model.CountryCode.Trim().ToUpperInvariant();
        player.DateOfBirth = model.DateOfBirth;
        player.Role = model.Role;
        player.Rating2 = model.Rating2;
        player.TotalMapsPlayed = model.TotalMapsPlayed;
        player.TeamId = model.TeamId;
        player.ImagePath = newImagePath ?? previousImagePath;
        if (imageUpload is not null)
        {
            ApplyImageMetadata(player, imageUpload);
        }

        _playerRepository.Update(player);
        DeletePlayerImageIfReplaced(previousImagePath, newImagePath);
        return RedirectToAction(nameof(DetailsBySlug), new { slug = RouteSlugHelper.ToRouteSegment(player.Nickname) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult UploadImage(int id, IFormFile? file)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return NotFound();
        }

        var player = _playerRepository.GetById(id);
        if (player is null)
        {
            return NotFound();
        }

        var validationError = GetPlayerImageValidationError(file);
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            return BadRequest(new { message = validationError });
        }

        var previousImagePath = player.ImagePath;
        var imageUpload = SavePlayerImage(file)!;
        var newImagePath = imageUpload.Path;
        ApplyImageMetadata(player, imageUpload);
        _playerRepository.Update(player);

        DeletePlayerImageIfReplaced(previousImagePath, newImagePath);
        return Ok(new { path = newImagePath });
    }

    [HttpGet("/players/{id:int}/image/search")]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult SearchImage(int id, string? query)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return NotFound();
        }

        var player = _playerRepository.GetById(id);
        if (player is null)
        {
            return NotFound();
        }

        return Json(MatchesUploadSearch(
            query,
            player.ImagePath,
            player.ImageContentType,
            player.ImageFileSize,
            player.ImageCreatedAtUtc)
            ? new[]
            {
                new
                {
                    path = player.ImagePath,
                    fileName = Path.GetFileName(player.ImagePath),
                    contentType = player.ImageContentType,
                    size = player.ImageFileSize,
                    createdAtUtc = player.ImageCreatedAtUtc
                }
            }
            : Array.Empty<object>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult DeleteImage(int id)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return NotFound();
        }

        var player = _playerRepository.GetById(id);
        if (player is null)
        {
            return NotFound();
        }

        var previousImagePath = player.ImagePath;
        player.ImagePath = null;
        player.ImageContentType = null;
        player.ImageFileSize = null;
        player.ImageCreatedAtUtc = null;
        _playerRepository.Update(player);
        DeletePlayerImage(previousImagePath);

        return Ok();
    }

    [HttpGet]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult Delete(int id)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return NotFound();
        }

        var player = _playerRepository.GetById(id);
        if (player is null)
        {
            return NotFound();
        }

        return View(player);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return NotFound();
        }

        var player = _playerRepository.GetById(id);
        if (player is null)
        {
            return NotFound();
        }

        _playerRepository.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/Player/Search")]
    public IActionResult Search(string query, int? currentTeamId = null)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
        {
            return Json(Array.Empty<object>());
        }

        var players = _playerRepository.SearchAvailableByNickname(query, currentTeamId, 8);
        return Json(players.Select(player => new { id = player.Id, text = player.Nickname, imagePath = player.ImagePath ?? string.Empty }));
    }

    private void ApplyFavoriteState(IEnumerable<Player> players)
    {
        var currentUser = GetCurrentForumUser();
        if (currentUser is null)
        {
            return;
        }

        var favoritePlayerIds = _forumRepository.GetFavoritePlayers(currentUser.Id).Select(player => player.Id).ToHashSet();
        foreach (var player in players)
        {
            player.IsFavorite = favoritePlayerIds.Contains(player.Id);
        }
    }

    private ForumUser? GetCurrentForumUser()
    {
        var userId = HttpContext.Session.GetInt32(ForumUserSessionKey);
        return userId.HasValue ? _forumRepository.GetForumUserById(userId.Value) : null;
    }

    private void PopulateTeams(int? selectedTeamId)
    {
        ViewBag.Teams = new SelectList(_teamRepository.GetAll().OrderBy(team => team.WorldRanking).ToList(), nameof(Team.Id), nameof(Team.Name), selectedTeamId);
    }

    private static Player MapToPlayer(PlayerCreateModel model)
    {
        return new Player
        {
            Nickname = model.Nickname.Trim(),
            FullName = model.FullName.Trim(),
            CountryCode = model.CountryCode.Trim().ToUpperInvariant(),
            DateOfBirth = model.DateOfBirth,
            Role = model.Role,
            Rating2 = model.Rating2,
            TotalMapsPlayed = model.TotalMapsPlayed,
            TeamId = model.TeamId,
            JoinedTeamAtUtc = DateTime.UtcNow
        };
    }

    private static PlayerEditModel MapToEditModel(Player player)
    {
        return new PlayerEditModel
        {
            Id = player.Id,
            Nickname = player.Nickname,
            FullName = player.FullName,
            CountryCode = player.CountryCode,
            DateOfBirth = player.DateOfBirth,
            Role = player.Role,
            Rating2 = player.Rating2,
            TotalMapsPlayed = player.TotalMapsPlayed,
            TeamId = player.TeamId,
            CurrentImagePath = player.ImagePath
        };
    }

    private void ValidatePlayer(PlayerCreateModel model, int? currentPlayerId = null)
    {
        var normalizedNickname = model.Nickname.Trim();
        var players = _playerRepository.GetAllAlphabetical();

        if (players.Any(player => player.Id != currentPlayerId && player.Nickname.Equals(normalizedNickname, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(model.Nickname), "A player with this nickname already exists.");
        }

        if (model.TeamId.HasValue && !_teamRepository.GetAll().Any(team => team.Id == model.TeamId.Value))
        {
            ModelState.AddModelError(nameof(model.TeamId), "The selected team could not be found.");
        }
    }

    private void ValidatePlayerImage(IFormFile? playerImage, string modelKey)
    {
        if (playerImage is null || playerImage.Length == 0)
        {
            return;
        }

        var validationError = GetPlayerImageValidationError(playerImage);
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            ModelState.AddModelError(modelKey, validationError);
        }
    }

    private static string? GetPlayerImageValidationError(IFormFile? playerImage)
    {
        if (playerImage is null || playerImage.Length == 0)
        {
            return "Select a PNG or WebP player image to upload.";
        }

        if (playerImage.Length > MaxPlayerImageBytes)
        {
            return "Player image must be 4 MB or smaller.";
        }

        var extension = Path.GetExtension(playerImage.FileName);
        var isPngExtension = extension.Equals(".png", StringComparison.OrdinalIgnoreCase);
        var isWebpExtension = extension.Equals(".webp", StringComparison.OrdinalIgnoreCase);
        if (!isPngExtension && !isWebpExtension)
        {
            return "Only PNG or WebP player images are allowed.";
        }

        if (!string.Equals(playerImage.ContentType, "image/png", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(playerImage.ContentType, "image/webp", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(playerImage.ContentType, "application/octet-stream", StringComparison.OrdinalIgnoreCase))
        {
            return "Only PNG or WebP player images are allowed.";
        }

        Span<byte> header = stackalloc byte[12];
        using var stream = playerImage.OpenReadStream();
        var bytesRead = stream.Read(header);
        if (isPngExtension)
        {
            return bytesRead >= PngSignature.Length && header[..PngSignature.Length].SequenceEqual(PngSignature)
                ? null
                : "The selected file is not a valid PNG image.";
        }

        return bytesRead >= 12 &&
               header[..RiffSignature.Length].SequenceEqual(RiffSignature) &&
               header[8..12].SequenceEqual(WebpSignature)
            ? null
            : "The selected file is not a valid WebP image.";
    }

    private UploadedFileMetadata? SavePlayerImage(IFormFile? playerImage)
    {
        if (playerImage is null || playerImage.Length == 0)
        {
            return null;
        }

        var uploadDirectory = Path.Combine(_environment.WebRootPath, "images", "players");
        Directory.CreateDirectory(uploadDirectory);

        var extension = Path.GetExtension(playerImage.FileName).Equals(".webp", StringComparison.OrdinalIgnoreCase)
            ? ".webp"
            : ".png";
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadDirectory, fileName);
        using var fileStream = System.IO.File.Create(filePath);
        playerImage.CopyTo(fileStream);

        return new UploadedFileMetadata(
            $"/images/players/{fileName}",
            playerImage.ContentType,
            playerImage.Length,
            DateTime.UtcNow);
    }

    private static void ApplyImageMetadata(Player player, UploadedFileMetadata upload)
    {
        player.ImagePath = upload.Path;
        player.ImageContentType = upload.ContentType;
        player.ImageFileSize = upload.Size;
        player.ImageCreatedAtUtc = upload.CreatedAtUtc;
    }

    private static bool MatchesUploadSearch(string? query, string? path, string? contentType, long? size, DateTime? createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(query))
        {
            return true;
        }

        var normalizedQuery = query.Trim();
        return path.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
               (contentType?.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (size?.ToString().Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (createdAtUtc?.ToString("u").Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private void DeletePlayerImageIfReplaced(string? previousImagePath, string? newImagePath)
    {
        if (string.IsNullOrWhiteSpace(previousImagePath) ||
            string.IsNullOrWhiteSpace(newImagePath) ||
            string.Equals(previousImagePath, newImagePath, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        DeletePlayerImage(previousImagePath);
    }

    private void DeletePlayerImage(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return;
        }

        var relativePath = imagePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(_environment.WebRootPath, relativePath));
        var playerImageRoot = Path.GetFullPath(Path.Combine(_environment.WebRootPath, "images", "players"));

        if (fullPath.StartsWith(playerImageRoot, StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(fullPath))
        {
            System.IO.File.Delete(fullPath);
        }
    }

    private sealed record UploadedFileMetadata(string Path, string ContentType, long Size, DateTime CreatedAtUtc);
}
