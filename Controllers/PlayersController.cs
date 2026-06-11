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
    private readonly IMatchRepository _matchRepository;
    private readonly IWebHostEnvironment _environment;
    private const string ForumUserSessionKey = "ForumUserId";

    public PlayersController(IPlayerRepository playerRepository, IForumRepository forumRepository, ITeamRepository teamRepository, IMatchRepository matchRepository, IWebHostEnvironment environment)
    {
        _playerRepository = playerRepository;
        _forumRepository = forumRepository;
        _teamRepository = teamRepository;
        _matchRepository = matchRepository;
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
        return View(BuildDetailsViewModel(player));
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
        return View("Details", BuildDetailsViewModel(player));
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
        var existingImage = GetExistingFile(GetPlayerImageRoot(), "/images/players", model.ExistingImagePath, [".png", ".webp"]);
        if (model.PlayerImage is null && !string.IsNullOrWhiteSpace(model.ExistingImagePath) && existingImage is null)
        {
            ModelState.AddModelError(nameof(model.ExistingImagePath), "The selected existing player image could not be found.");
        }

        if (!ModelState.IsValid)
        {
            PopulateTeams(model.TeamId);
            return View(model);
        }

        var player = MapToPlayer(model);
        var imageUpload = SavePlayerImage(model.PlayerImage) ?? existingImage;
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
        return RedirectToAction(nameof(DetailsBySlug), new { slug = RouteSlugHelper.ToRouteSegment(player.Nickname) });
    }

    [HttpPost("/players/{id:int}/image")]
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

        var imageUpload = SavePlayerImage(file)!;
        var newImagePath = imageUpload.Path;
        ApplyImageMetadata(player, imageUpload);
        _playerRepository.Update(player);

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

        if (!string.IsNullOrWhiteSpace(player.ImagePath))
        {
            return Json(Array.Empty<object>());
        }

        return Json(SearchFiles(GetPlayerImageRoot(), "/images/players", query, [".png", ".webp"]));
    }

    [HttpGet("/players/image/search")]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult SearchAvailableImages(string? query)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return NotFound();
        }

        return Json(SearchFiles(GetPlayerImageRoot(), "/images/players", query, [".png", ".webp"]));
    }

    [HttpPost("/players/{id:int}/image/attach")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult AttachImage(int id, [FromForm] string? path)
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

        var file = GetExistingFile(GetPlayerImageRoot(), "/images/players", path, [".png", ".webp"]);
        if (file is null)
        {
            return BadRequest(new { message = "The selected player image could not be found." });
        }

        ApplyImageMetadata(player, file);
        _playerRepository.Update(player);
        return Ok(new { path = file.Path });
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

        player.ImagePath = null;
        player.ImageContentType = null;
        player.ImageFileSize = null;
        player.ImageCreatedAtUtc = null;
        _playerRepository.Update(player);
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

    private PlayerDetailsViewModel BuildDetailsViewModel(Player player)
    {
        var upcomingMatches = player.TeamId.HasValue
            ? _matchRepository.GetAll()
                .Where(match => !match.IsFinished &&
                    match.ScheduledAtUtc >= DateTime.UtcNow &&
                    (match.TeamAId == player.TeamId.Value || match.TeamBId == player.TeamId.Value))
                .OrderBy(match => match.ScheduledAtUtc)
                .Take(5)
                .ToList()
            : [];

        return new PlayerDetailsViewModel
        {
            Player = player,
            UpcomingMatches = upcomingMatches
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
        var fileName = GetAvailableFileName(uploadDirectory, playerImage.FileName, extension);
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

    private string GetPlayerImageRoot()
    {
        return Path.Combine(_environment.WebRootPath, "images", "players");
    }

    private sealed record UploadedFileMetadata(string Path, string ContentType, long Size, DateTime CreatedAtUtc);

    private static IReadOnlyList<object> SearchFiles(string root, string webRoot, string? query, string[] allowedExtensions)
    {
        Directory.CreateDirectory(root);
        var normalizedQuery = query?.Trim() ?? string.Empty;
        if (normalizedQuery.Length == 1)
        {
            return [];
        }

        var files = Directory.EnumerateFiles(root)
            .Where(file => allowedExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
            .Select(file => new FileInfo(file))
            .Where(file => normalizedQuery.Length == 0 ||
                           file.Name.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase));

        files = normalizedQuery.Length == 0
            ? files.OrderByDescending(file => file.CreationTimeUtc).Take(5)
            : files.OrderBy(file => file.Name).Take(20);

        return files
            .Select(file => new
            {
                path = $"{webRoot}/{file.Name}",
                fileName = file.Name,
                contentType = GetContentType(file.Extension),
                size = file.Length,
                createdAtUtc = file.CreationTimeUtc
            })
            .Cast<object>()
            .ToList();
    }

    private static UploadedFileMetadata? GetExistingFile(string root, string webRoot, string? path, string[] allowedExtensions)
    {
        var fileName = Path.GetFileName(path);
        if (string.IsNullOrWhiteSpace(fileName) || !allowedExtensions.Contains(Path.GetExtension(fileName), StringComparer.OrdinalIgnoreCase))
        {
            return null;
        }

        var fullPath = Path.GetFullPath(Path.Combine(root, fileName));
        var fullRoot = Path.GetFullPath(root);
        if (!fullPath.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(fullPath))
        {
            return null;
        }

        var file = new FileInfo(fullPath);
        return new UploadedFileMetadata($"{webRoot}/{file.Name}", GetContentType(file.Extension), file.Length, file.CreationTimeUtc);
    }

    private static string GetAvailableFileName(string directory, string originalName, string fallbackExtension)
    {
        var stem = string.Concat(Path.GetFileNameWithoutExtension(originalName).Select(character =>
            Path.GetInvalidFileNameChars().Contains(character) ? '-' : character)).Trim(' ', '.', '-');
        stem = string.IsNullOrWhiteSpace(stem) ? "upload" : stem;
        var extension = string.IsNullOrWhiteSpace(Path.GetExtension(originalName)) ? fallbackExtension : Path.GetExtension(originalName).ToLowerInvariant();
        var fileName = $"{stem}{extension}";
        var suffix = 2;
        while (System.IO.File.Exists(Path.Combine(directory, fileName)))
        {
            fileName = $"{stem}-{suffix++}{extension}";
        }

        return fileName;
    }

    private static string GetContentType(string extension)
    {
        return extension.Equals(".webp", StringComparison.OrdinalIgnoreCase) ? "image/webp" : "image/png";
    }
}
