using cs2_esports.Data;
using cs2_esports.Helpers;
using cs2_esports.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cs2_esports.Controllers;

[Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
[Route("admin/files")]
public class AdminFilesController : Controller
{
    private readonly Cs2ScopeDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;

    public AdminFilesController(Cs2ScopeDbContext dbContext, IWebHostEnvironment environment)
    {
        _dbContext = dbContext;
        _environment = environment;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var eventAssignmentRows = await _dbContext.Tournaments
            .AsNoTracking()
            .Where(eventItem => eventItem.BannerImagePath != null)
            .Select(eventItem => new { Path = eventItem.BannerImagePath!, Name = eventItem.Name })
            .ToListAsync();
        var eventAssignments = eventAssignmentRows
            .GroupBy(item => item.Path)
            .ToDictionary(group => group.Key, group => group.Select(item => item.Name).OrderBy(name => name).ToList());

        var playerAssignmentRows = await _dbContext.Players
            .AsNoTracking()
            .Where(player => player.ImagePath != null)
            .Select(player => new { Path = player.ImagePath!, Name = player.Nickname })
            .ToListAsync();
        var playerAssignments = playerAssignmentRows
            .GroupBy(item => item.Path)
            .ToDictionary(group => group.Key, group => group.Select(item => item.Name).OrderBy(name => name).ToList());

        return View(new AdminFilesIndexViewModel
        {
            EventFiles = GetFiles("event", GetEventRoot(), "/images/events/banners", [".png"], eventAssignments),
            PlayerFiles = GetFiles("player", GetPlayerRoot(), "/images/players", [".png", ".webp"], playerAssignments)
        });
    }

    [HttpPost("delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string? kind, string? path)
    {
        var file = ResolveFile(kind, path);
        if (file is null)
        {
            return BadRequest();
        }

        if (string.Equals(kind, "event", StringComparison.OrdinalIgnoreCase))
        {
            var events = await _dbContext.Tournaments.Where(eventItem => eventItem.BannerImagePath == file.WebPath).ToListAsync();
            foreach (var eventItem in events)
            {
                eventItem.BannerImagePath = null;
                eventItem.BannerContentType = null;
                eventItem.BannerFileSize = null;
                eventItem.BannerCreatedAtUtc = null;
            }
        }
        else
        {
            var players = await _dbContext.Players.Where(player => player.ImagePath == file.WebPath).ToListAsync();
            foreach (var player in players)
            {
                player.ImagePath = null;
                player.ImageContentType = null;
                player.ImageFileSize = null;
                player.ImageCreatedAtUtc = null;
            }
        }

        await _dbContext.SaveChangesAsync();
        System.IO.File.Delete(file.FullPath);
        TempData["AdminFilesMessage"] = $"Deleted {file.FileName}.";
        return RedirectToAction(nameof(Index));
    }

    private List<AdminFileItemViewModel> GetFiles(
        string kind,
        string root,
        string webRoot,
        string[] allowedExtensions,
        IReadOnlyDictionary<string, List<string>> assignments)
    {
        Directory.CreateDirectory(root);

        return Directory.EnumerateFiles(root)
            .Where(path => allowedExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
            .Select(path => new FileInfo(path))
            .OrderByDescending(file => file.CreationTimeUtc)
            .Select(file =>
            {
                var webPath = $"{webRoot}/{file.Name}";
                return new AdminFileItemViewModel
                {
                    Kind = kind,
                    FileName = file.Name,
                    WebPath = webPath,
                    ContentType = GetContentType(file.Extension),
                    FileSize = file.Length,
                    CreatedAtUtc = file.CreationTimeUtc,
                    AssignedEntities = assignments.TryGetValue(webPath, out var names) ? names : []
                };
            })
            .ToList();
    }

    private ResolvedAdminFile? ResolveFile(string? kind, string? path)
    {
        var isEvent = string.Equals(kind, "event", StringComparison.OrdinalIgnoreCase);
        var isPlayer = string.Equals(kind, "player", StringComparison.OrdinalIgnoreCase);
        if (!isEvent && !isPlayer)
        {
            return null;
        }

        var root = isEvent ? GetEventRoot() : GetPlayerRoot();
        var webRoot = isEvent ? "/images/events/banners" : "/images/players";
        var allowedExtensions = isEvent ? new[] { ".png" } : [".png", ".webp"];
        var fileName = Path.GetFileName(path);
        if (string.IsNullOrWhiteSpace(fileName) || !allowedExtensions.Contains(Path.GetExtension(fileName), StringComparer.OrdinalIgnoreCase))
        {
            return null;
        }

        var fullRoot = Path.GetFullPath(root);
        var fullPath = Path.GetFullPath(Path.Combine(fullRoot, fileName));
        if (!fullPath.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(fullPath))
        {
            return null;
        }

        return new ResolvedAdminFile(fullPath, $"{webRoot}/{fileName}", fileName);
    }

    private string GetEventRoot() => Path.Combine(_environment.WebRootPath, "images", "events", "banners");

    private string GetPlayerRoot() => Path.Combine(_environment.WebRootPath, "images", "players");

    private static string GetContentType(string extension) =>
        extension.Equals(".webp", StringComparison.OrdinalIgnoreCase) ? "image/webp" : "image/png";

    private sealed record ResolvedAdminFile(string FullPath, string WebPath, string FileName);
}
