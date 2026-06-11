using cs2_esports.Data;
using cs2_esports.Helpers;
using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace cs2_esports.Controllers;

public class EventsController : Controller
{
    private const long MaxBannerImageBytes = 4 * 1024 * 1024;
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    private readonly IEventRepository _eventRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly Cs2ScopeDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;

    public EventsController(IEventRepository eventRepository, ITeamRepository teamRepository, Cs2ScopeDbContext dbContext, IWebHostEnvironment environment)
    {
        _eventRepository = eventRepository;
        _teamRepository = teamRepository;
        _dbContext = dbContext;
        _environment = environment;
    }

    public IActionResult Index()
    {
        var eventsData = _eventRepository.GetAll();
        return View(eventsData);
    }

    public IActionResult Details(int id)
    {
        var eventItem = _eventRepository.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        return View(eventItem);
    }

    [HttpGet("/event/{slug}")]
    public IActionResult DetailsBySlug(string slug)
    {
        var eventSummary = _eventRepository.GetAll().FirstOrDefault(tournament =>
            RouteSlugHelper.MatchesRouteSegment(tournament.Name, slug));

        if (eventSummary is null)
        {
            return NotFound();
        }

        var eventItem = _eventRepository.GetById(eventSummary.Id);
        if (eventItem is null)
        {
            return NotFound();
        }

        return View("Details", eventItem);
    }

    [HttpGet]
    [Authorize(Roles = EventRoleHelper.EventAdminRoles)]
    public IActionResult Create()
    {
        var model = new EventCreateModel
        {
            StartDateUtc = DateTime.UtcNow,
            EndDateUtc = DateTime.UtcNow.AddDays(3),
            Tier = EventTier.S,
            Organizer = EventRoleHelper.GetDefaultOrganizerForAdmin(User)
        };

        PopulateViewData(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = EventRoleHelper.EventAdminRoles)]
    public IActionResult Create(EventCreateModel model)
    {
        var lockedOrganizer = EventRoleHelper.GetDefaultOrganizerForAdmin(User);
        if (!string.IsNullOrWhiteSpace(lockedOrganizer))
        {
            model.Organizer = lockedOrganizer;
        }

        if (!EventRoleHelper.CanManageOrganizer(User, model.Organizer))
        {
            return Forbid();
        }

        NormalizeSelectedTeams(model);
        ValidateEventDates(model);
        ValidateEventUniqueness(model);
        ValidateBannerImage(model.BannerImage);
        var existingBanner = GetExistingFile(GetBannerRoot(), "/images/events/banners", model.ExistingBannerPath, [".png"]);
        if (model.BannerImage is null && !string.IsNullOrWhiteSpace(model.ExistingBannerPath) && existingBanner is null)
        {
            ModelState.AddModelError(nameof(model.ExistingBannerPath), "The selected existing banner could not be found.");
        }

        if (!ModelState.IsValid)
        {
            PopulateViewData(model);
            return View(model);
        }

        var bannerUpload = SaveBannerImage(model.BannerImage) ?? existingBanner;

        var eventItem = new Event
        {
            Name = model.Name.Trim(),
            Organizer = model.Organizer.Trim(),
            Tier = model.Tier,
            PrizePoolUsd = model.PrizePoolUsd,
            StartDateUtc = model.StartDateUtc,
            EndDateUtc = model.EndDateUtc,
            IsLan = model.IsLan,
            BannerImagePath = bannerUpload?.Path,
            BannerContentType = bannerUpload?.ContentType,
            BannerFileSize = bannerUpload?.Size,
            BannerCreatedAtUtc = bannerUpload?.CreatedAtUtc,
            EventVenueId = model.EventVenueId,
            AdminUserId = GetCurrentAdminUserId(),
            Teams = _teamRepository.GetByIds(model.SelectedTeamIds).ToList()
        };

        _eventRepository.Add(eventItem);
        return RedirectToAction(nameof(DetailsBySlug), new { slug = RouteSlugHelper.ToRouteSegment(eventItem.Name) });
    }

    [HttpGet]
    [Authorize(Roles = EventRoleHelper.EventAdminRoles)]
    public IActionResult Edit(int id)
    {
        var eventItem = _eventRepository.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        if (!EventRoleHelper.CanManageOrganizer(User, eventItem.Organizer))
        {
            return Forbid();
        }

        var model = MapToEditModel(eventItem);
        PopulateViewData(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = EventRoleHelper.EventAdminRoles)]
    public IActionResult Edit(EventEditModel model)
    {
        var eventItem = _eventRepository.GetById(model.Id);
        if (eventItem is null)
        {
            return NotFound();
        }

        if (!EventRoleHelper.CanManageOrganizer(User, eventItem.Organizer) || !EventRoleHelper.CanManageOrganizer(User, model.Organizer))
        {
            return Forbid();
        }

        NormalizeSelectedTeams(model);
        ValidateEventDates(model);
        ValidateEventUniqueness(model, model.Id);
        ValidateBannerImage(model.BannerImage);

        if (!ModelState.IsValid)
        {
            model.CurrentBannerImagePath = eventItem.BannerImagePath;
            PopulateViewData(model);
            return View(model);
        }

        var previousBannerImagePath = eventItem.BannerImagePath;
        var bannerUpload = SaveBannerImage(model.BannerImage);
        var newBannerImagePath = bannerUpload?.Path;

        eventItem.Name = model.Name.Trim();
        eventItem.Organizer = model.Organizer.Trim();
        eventItem.Tier = model.Tier;
        eventItem.PrizePoolUsd = model.PrizePoolUsd;
        eventItem.StartDateUtc = model.StartDateUtc;
        eventItem.EndDateUtc = model.EndDateUtc;
        eventItem.IsLan = model.IsLan;
        eventItem.BannerImagePath = newBannerImagePath ?? previousBannerImagePath;
        if (bannerUpload is not null)
        {
            ApplyBannerMetadata(eventItem, bannerUpload);
        }
        eventItem.EventVenueId = model.EventVenueId;
        eventItem.AdminUserId = GetCurrentAdminUserId();
        eventItem.Teams = _teamRepository.GetByIds(model.SelectedTeamIds).ToList();

        _eventRepository.Update(eventItem);
        return RedirectToAction(nameof(DetailsBySlug), new { slug = RouteSlugHelper.ToRouteSegment(eventItem.Name) });
    }

    [HttpPost("/events/{id:int}/banner")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = EventRoleHelper.EventAdminRoles)]
    public IActionResult UploadBanner(int id, IFormFile? file)
    {
        var eventItem = _eventRepository.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        if (!EventRoleHelper.CanManageOrganizer(User, eventItem.Organizer))
        {
            return Forbid();
        }

        var validationError = GetBannerImageValidationError(file);
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            return BadRequest(new { message = validationError });
        }

        var bannerUpload = SaveBannerImage(file)!;
        var newBannerImagePath = bannerUpload.Path;
        ApplyBannerMetadata(eventItem, bannerUpload);
        eventItem.AdminUserId = GetCurrentAdminUserId();

        _eventRepository.Update(eventItem);

        return Ok(new { path = newBannerImagePath });
    }

    [HttpGet("/events/{id:int}/banner/search")]
    [Authorize(Roles = EventRoleHelper.EventAdminRoles)]
    public IActionResult SearchBanner(int id, string? query)
    {
        var eventItem = _eventRepository.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        if (!EventRoleHelper.CanManageOrganizer(User, eventItem.Organizer))
        {
            return Forbid();
        }

        if (!string.IsNullOrWhiteSpace(eventItem.BannerImagePath))
        {
            return Json(Array.Empty<object>());
        }

        return Json(SearchFiles(GetBannerRoot(), "/images/events/banners", query, [".png"]));
    }

    [HttpGet("/events/banner/search")]
    [Authorize(Roles = EventRoleHelper.EventAdminRoles)]
    public IActionResult SearchAvailableBanners(string? query)
    {
        return Json(SearchFiles(GetBannerRoot(), "/images/events/banners", query, [".png"]));
    }

    [HttpPost("/events/{id:int}/banner/attach")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = EventRoleHelper.EventAdminRoles)]
    public IActionResult AttachBanner(int id, [FromForm] string? path)
    {
        var eventItem = _eventRepository.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        if (!EventRoleHelper.CanManageOrganizer(User, eventItem.Organizer))
        {
            return Forbid();
        }

        var file = GetExistingFile(GetBannerRoot(), "/images/events/banners", path, [".png"]);
        if (file is null)
        {
            return BadRequest(new { message = "The selected banner file could not be found." });
        }

        ApplyBannerMetadata(eventItem, file);
        eventItem.AdminUserId = GetCurrentAdminUserId();
        _eventRepository.Update(eventItem);
        return Ok(new { path = file.Path });
    }

    [HttpPost("/events/{id:int}/banner/delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = EventRoleHelper.EventAdminRoles)]
    public IActionResult DeleteBanner(int id)
    {
        var eventItem = _eventRepository.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        if (!EventRoleHelper.CanManageOrganizer(User, eventItem.Organizer))
        {
            return Forbid();
        }

        eventItem.BannerImagePath = null;
        eventItem.BannerContentType = null;
        eventItem.BannerFileSize = null;
        eventItem.BannerCreatedAtUtc = null;
        eventItem.AdminUserId = GetCurrentAdminUserId();

        _eventRepository.Update(eventItem);
        return Ok(new { deleted = true });
    }

    [HttpGet]
    [Authorize(Roles = EventRoleHelper.EventAdminRoles)]
    public IActionResult Delete(int id)
    {
        var eventItem = _eventRepository.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        if (!EventRoleHelper.CanManageOrganizer(User, eventItem.Organizer))
        {
            return Forbid();
        }

        return View(eventItem);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    [Authorize(Roles = EventRoleHelper.EventAdminRoles)]
    public IActionResult DeleteConfirmed(int id)
    {
        var eventItem = _eventRepository.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        if (!EventRoleHelper.CanManageOrganizer(User, eventItem.Organizer))
        {
            return Forbid();
        }

        _eventRepository.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/Event/Search")]
    public IActionResult Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
        {
            return Json(Array.Empty<object>());
        }

        var eventsData = _eventRepository.GetAll()
            .Where(eventItem => eventItem.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(eventItem => eventItem.StartDateUtc)
            .Take(8)
            .Select(eventItem => new { id = eventItem.Id, text = eventItem.Name });

        return Json(eventsData);
    }

    private void PopulateViewData(EventCreateModel model)
    {
        ViewBag.EventVenues = new SelectList(
            _dbContext.EventVenues.OrderBy(venue => venue.Name).ToList(),
            nameof(EventVenue.Id),
            nameof(EventVenue.Name),
            model.EventVenueId);
        ViewBag.LockOrganizer = !string.IsNullOrWhiteSpace(EventRoleHelper.GetDefaultOrganizerForAdmin(User));
    }

    private void ValidateEventUniqueness(EventCreateModel model, int? currentEventId = null)
    {
        var normalizedName = model.Name.Trim();
        var eventsData = _eventRepository.GetAll();

        if (eventsData.Any(eventItem => eventItem.Id != currentEventId && eventItem.Name.Equals(normalizedName, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(model.Name), "An event with this name already exists.");
        }
    }

    private void ValidateEventDates(EventCreateModel model)
    {
        if (model.EndDateUtc < model.StartDateUtc)
        {
            ModelState.AddModelError(nameof(model.EndDateUtc), "End date must be on or after the start date.");
        }
    }

    private void NormalizeSelectedTeams(EventCreateModel model)
    {
        model.SelectedTeamIds = model.SelectedTeamIds
            .Where(teamId => teamId > 0)
            .Distinct()
            .ToList();

        var selectedTeams = _teamRepository.GetByIds(model.SelectedTeamIds);
        if (selectedTeams.Count != model.SelectedTeamIds.Count)
        {
            ModelState.AddModelError(nameof(model.SelectedTeamIds), "One or more selected teams could not be found.");
        }

        model.SelectedTeams = selectedTeams
            .Select(team => new TeamAutocompleteItemModel
            {
                Id = team.Id,
                Text = team.Name,
                    LogoPath = TeamLogoResolver.GetLogoPath(team.Name, team.Tag) ?? string.Empty
            })
            .ToList();
    }

    private EventEditModel MapToEditModel(Event eventItem)
    {
        return new EventEditModel
        {
            Id = eventItem.Id,
            Name = eventItem.Name,
            Organizer = eventItem.Organizer,
            Tier = eventItem.Tier,
            PrizePoolUsd = eventItem.PrizePoolUsd,
            StartDateUtc = eventItem.StartDateUtc,
            EndDateUtc = eventItem.EndDateUtc,
            IsLan = eventItem.IsLan,
            CurrentBannerImagePath = eventItem.BannerImagePath,
            EventVenueId = eventItem.EventVenueId,
            SelectedTeamIds = eventItem.Teams.OrderBy(team => team.WorldRanking).Select(team => team.Id).ToList(),
            SelectedTeams = eventItem.Teams.OrderBy(team => team.WorldRanking).Select(team => new TeamAutocompleteItemModel
            {
                Id = team.Id,
                Text = team.Name,
                    LogoPath = TeamLogoResolver.GetLogoPath(team.Name, team.Tag) ?? string.Empty
            }).ToList()
        };
    }

    private void ValidateBannerImage(IFormFile? bannerImage)
    {
        if (bannerImage is null || bannerImage.Length == 0)
        {
            return;
        }

        var validationError = GetBannerImageValidationError(bannerImage);
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            ModelState.AddModelError(nameof(EventCreateModel.BannerImage), validationError);
        }
    }

    private static string? GetBannerImageValidationError(IFormFile? bannerImage)
    {
        if (bannerImage is null || bannerImage.Length == 0)
        {
            return "Select a PNG banner image to upload.";
        }

        if (bannerImage.Length > MaxBannerImageBytes)
        {
            return "Banner image must be 4 MB or smaller.";
        }

        if (!Path.GetExtension(bannerImage.FileName).Equals(".png", StringComparison.OrdinalIgnoreCase))
        {
            return "Only PNG banner images are allowed.";
        }

        if (!string.Equals(bannerImage.ContentType, "image/png", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(bannerImage.ContentType, "application/octet-stream", StringComparison.OrdinalIgnoreCase))
        {
            return "Only PNG banner images are allowed.";
        }

        Span<byte> header = stackalloc byte[PngSignature.Length];
        using var stream = bannerImage.OpenReadStream();
        var bytesRead = stream.Read(header);
        return bytesRead == PngSignature.Length && header.SequenceEqual(PngSignature)
            ? null
            : "The selected file is not a valid PNG image.";
    }

    private UploadedFileMetadata? SaveBannerImage(IFormFile? bannerImage)
    {
        if (bannerImage is null || bannerImage.Length == 0)
        {
            return null;
        }

        var uploadDirectory = Path.Combine(_environment.WebRootPath, "images", "events", "banners");
        Directory.CreateDirectory(uploadDirectory);

        var fileName = GetAvailableFileName(uploadDirectory, bannerImage.FileName, ".png");
        var filePath = Path.Combine(uploadDirectory, fileName);
        using var fileStream = System.IO.File.Create(filePath);
        bannerImage.CopyTo(fileStream);

        return new UploadedFileMetadata(
            $"/images/events/banners/{fileName}",
            bannerImage.ContentType,
            bannerImage.Length,
            DateTime.UtcNow);
    }

    private static void ApplyBannerMetadata(Event eventItem, UploadedFileMetadata upload)
    {
        eventItem.BannerImagePath = upload.Path;
        eventItem.BannerContentType = upload.ContentType;
        eventItem.BannerFileSize = upload.Size;
        eventItem.BannerCreatedAtUtc = upload.CreatedAtUtc;
    }

    private string GetBannerRoot()
    {
        return Path.Combine(_environment.WebRootPath, "images", "events", "banners");
    }

    private int? GetCurrentAdminUserId()
    {
        var username = User.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(username))
        {
            var identityAdminUserId = _dbContext.Users
                .Where(user => user.UserName == username)
                .Select(user => user.LegacyAdminUserId)
                .FirstOrDefault();

            if (identityAdminUserId.HasValue)
            {
                return identityAdminUserId;
            }
        }

        return HttpContext.Session.GetInt32(AuthSessionKeys.AdminUserId);
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
