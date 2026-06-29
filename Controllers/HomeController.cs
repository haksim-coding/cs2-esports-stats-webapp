using cs2_esports.Models;
using cs2_esports.Data;
using cs2_esports.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using cs2_esports.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace cs2_esports.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventRepository _eventRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IForumRepository _forumRepository;
        private readonly Cs2ScopeDbContext _dbContext;

        public HomeController(
            IEventRepository eventRepository,
            IPlayerRepository playerRepository,
            ITeamRepository teamRepository,
            IForumRepository forumRepository,
            Cs2ScopeDbContext dbContext)
        {
            _eventRepository = eventRepository;
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
            _forumRepository = forumRepository;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var loggedInUser = GetCurrentUser();
            var loggedInRole = GetLoggedInRole();
            var nowUtc = DateTime.UtcNow;

            var upcomingEvents = _eventRepository.GetAll()
                .Where(e => e.StartDateUtc >= nowUtc)
                .OrderBy(e => e.StartDateUtc)
                .Take(3)
                .ToList();

            if (upcomingEvents.Count == 0)
            {
                upcomingEvents = _eventRepository.GetAll()
                    .OrderBy(e => e.StartDateUtc)
                    .Take(3)
                    .ToList();
            }

            var model = new HomeViewModel
            {
                UpcomingEvents = upcomingEvents,

                TopPlayers = _playerRepository.GetAllAlphabetical()
                    .OrderByDescending(p => p.Rating2)
                    .Take(3)
                    .ToList(),

                TopTeams = _teamRepository.GetAll()
                    .Where(t => t.WorldRanking > 0)
                    .OrderBy(t => t.WorldRanking)
                    .Take(3)
                    .ToList(),

                LoggedInUser = loggedInUser,
                LoggedInUserRoleLabel = loggedInRole is null ? null : EventRoleHelper.GetRoleLabel(loggedInRole),
                LoggedInUserRoleBadgeClass = GetLoggedInRoleBadgeClass(loggedInRole),
                CanCreateForumPost = loggedInUser is ForumUser
            };

            return View(model);
        }

        [HttpGet("/search")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Search(string? query)
        {
            var normalizedQuery = query?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedQuery) || normalizedQuery.Length < 2)
            {
                return Json(new
                {
                    pages = Array.Empty<object>(),
                    players = Array.Empty<object>(),
                    teams = Array.Empty<object>(),
                    events = Array.Empty<object>(),
                    total = 0
                });
            }

            normalizedQuery = normalizedQuery[..Math.Min(normalizedQuery.Length, 80)];
            var searchTerm = normalizedQuery.ToLower();
            var hasLoggedInUser = GetCurrentUser() is not null;

            var pages = new[]
                {
                    new { title = "Home", subtitle = "Dashboard and featured CS2 data", meta = "Main menu", url = Url.Action("Index", "Home") ?? "/", icon = "⌂", keywords = "home dashboard overview main" },
                    new { title = "Teams", subtitle = "Team rankings, rosters, and results", meta = "Main menu", url = Url.Action("Index", "Teams") ?? "/Teams", icon = "T", keywords = "teams clubs rankings rosters main menu" },
                    new { title = "Events", subtitle = "Tournaments, venues, and prize pools", meta = "Main menu", url = Url.Action("Index", "Events") ?? "/Events", icon = "E", keywords = "events tournaments competitions venues main menu" },
                    new { title = "Matches", subtitle = "Scheduled and completed matches", meta = "Main menu", url = Url.Action("Index", "Matches") ?? "/Matches", icon = "M", keywords = "matches games scores results schedule main menu" },
                    new { title = "Players", subtitle = "Player profiles, roles, and ratings", meta = "Main menu", url = Url.Action("Index", "Players") ?? "/Players", icon = "P", keywords = "players profiles ratings roles main menu" },
                    new { title = "Forums", subtitle = "Community discussions and event threads", meta = "Main menu", url = Url.Action("Index", "Forums") ?? "/Forums", icon = "F", keywords = "forums community discussions posts threads main menu" },
                    new { title = "Login", subtitle = "Sign in to your CS2Scope account", meta = "Account page", url = Url.Action("Login", "Auth") ?? "/login", icon = "→", keywords = "login sign in account authentication page" },
                    new { title = "Register", subtitle = "Create a CS2Scope account", meta = "Account page", url = Url.Action("Register", "Auth") ?? "/register", icon = "+", keywords = "register sign up create account page" },
                    new { title = "My Profile", subtitle = "Profile details and saved favorites", meta = "Account page", url = Url.Action("Profile", "Auth") ?? "/my-profile", icon = "●", keywords = "profile account favorites saved teams players page" }
                }
                .Where(page => page.title != "My Profile" || hasLoggedInUser)
                .Where(page =>
                    page.title.ToLower().Contains(searchTerm) ||
                    page.subtitle.ToLower().Contains(searchTerm) ||
                    page.keywords.Contains(searchTerm))
                .OrderByDescending(page => page.title.ToLower() == searchTerm)
                .ThenByDescending(page => page.title.ToLower().StartsWith(searchTerm))
                .ThenBy(page => page.title)
                .Take(8)
                .Select(page => new
                {
                    page.title,
                    page.subtitle,
                    page.meta,
                    page.url,
                    page.icon
                })
                .ToList();

            var playerMatches = _dbContext.Players
                .AsNoTracking()
                .Include(player => player.Team)
                .Where(player =>
                    player.Nickname.ToLower().Contains(searchTerm) ||
                    player.FullName.ToLower().Contains(searchTerm))
                .OrderByDescending(player => player.Nickname.ToLower() == searchTerm)
                .ThenByDescending(player => player.Nickname.ToLower().StartsWith(searchTerm))
                .ThenBy(player => player.Nickname)
                .Take(6)
                .ToList();

            var teamMatches = _dbContext.Teams
                .AsNoTracking()
                .Where(team =>
                    team.Name.ToLower().Contains(searchTerm) ||
                    team.Tag.ToLower().Contains(searchTerm))
                .OrderByDescending(team => team.Name.ToLower() == searchTerm || team.Tag.ToLower() == searchTerm)
                .ThenByDescending(team => team.Name.ToLower().StartsWith(searchTerm) || team.Tag.ToLower().StartsWith(searchTerm))
                .ThenBy(team => team.WorldRanking <= 0 ? int.MaxValue : team.WorldRanking)
                .ThenBy(team => team.Name)
                .Take(6)
                .ToList();

            var eventMatches = _dbContext.Tournaments
                .AsNoTracking()
                .Where(eventItem =>
                    eventItem.Name.ToLower().Contains(searchTerm) ||
                    eventItem.Organizer.ToLower().Contains(searchTerm))
                .OrderByDescending(eventItem => eventItem.Name.ToLower() == searchTerm)
                .ThenByDescending(eventItem => eventItem.Name.ToLower().StartsWith(searchTerm))
                .ThenByDescending(eventItem => eventItem.EndDateUtc >= DateTime.UtcNow)
                .ThenBy(eventItem => eventItem.StartDateUtc)
                .Take(6)
                .ToList();

            var players = playerMatches.Select(player => new
            {
                id = player.Id,
                title = player.Nickname,
                subtitle = player.FullName,
                meta = player.Team is null ? "Free agent" : $"Playing for {player.Team.Name}",
                url = Url.Action("DetailsBySlug", "Players", new { slug = RouteSlugHelper.ToRouteSegment(player.Nickname) }),
                imageUrl = Url.Content(string.IsNullOrWhiteSpace(player.ImagePath) ? "~/images/default-avatar.svg" : player.ImagePath),
                flagUrl = CountryFlagHelper.GetFlagImageUrl(player.CountryCode),
                countryCode = player.CountryCode,
                badge = player.Team?.Tag
            }).ToList();

            var teams = teamMatches.Select(team => new
            {
                id = team.Id,
                title = team.Name,
                subtitle = team.Tag,
                meta = team.WorldRanking > 0 ? $"World ranking #{team.WorldRanking}" : "Unranked",
                url = Url.Action("DetailsBySlug", "Teams", new { slug = RouteSlugHelper.ToRouteSegment(team.Name) }),
                imageUrl = TeamLogoResolver.GetLogoPath(team.Name, team.Tag) is { } logoPath ? Url.Content(logoPath) : null,
                flagUrl = CountryFlagHelper.GetFlagImageUrl(team.CountryCode),
                countryCode = team.CountryCode,
                fallback = TeamLogoResolver.GetBadgeText(team.Name, team.Tag)
            }).ToList();

            var eventsData = eventMatches.Select(eventItem => new
            {
                id = eventItem.Id,
                title = eventItem.Name,
                subtitle = eventItem.Organizer,
                meta = $"{eventItem.StartDateUtc:MMM d} - {eventItem.EndDateUtc:MMM d, yyyy}",
                url = Url.Action("DetailsBySlug", "Events", new { slug = RouteSlugHelper.ToRouteSegment(eventItem.Name) }),
                imageUrl = string.IsNullOrWhiteSpace(eventItem.BannerImagePath) ? null : Url.Content(eventItem.BannerImagePath),
                badge = eventItem.Tier == EventTier.Major ? "Major" : $"{eventItem.Tier} Tier",
                isLive = eventItem.StartDateUtc <= DateTime.UtcNow && eventItem.EndDateUtc >= DateTime.UtcNow
            }).ToList();

            return Json(new
            {
                pages,
                players,
                teams,
                events = eventsData,
                total = pages.Count + players.Count + teams.Count + eventsData.Count
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private User? GetCurrentUser()
        {
            var userType = HttpContext.Session.GetString(AuthSessionKeys.UserType);
            if (string.Equals(userType, AuthSessionKeys.ForumUserType, StringComparison.Ordinal))
            {
                var userId = HttpContext.Session.GetInt32(AuthSessionKeys.ForumUserId);
                return userId.HasValue ? _forumRepository.GetForumUserById(userId.Value) : null;
            }

            if (string.Equals(userType, AuthSessionKeys.AdminUserType, StringComparison.Ordinal))
            {
                if (!EventRoleHelper.IsEventAdmin(User))
                {
                    return null;
                }

                var userId = HttpContext.Session.GetInt32(AuthSessionKeys.AdminUserId);
                return userId.HasValue ? _dbContext.AdminUsers.FirstOrDefault(user => user.Id == userId.Value) : null;
            }

            return null;
        }

        private string? GetLoggedInRole()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            if (User.IsInRole(EventRoleHelper.SuperAdminRole))
            {
                return EventRoleHelper.SuperAdminRole;
            }

            if (User.IsInRole(EventRoleHelper.BlastAdminRole))
            {
                return EventRoleHelper.BlastAdminRole;
            }

            if (User.IsInRole(EventRoleHelper.EslAdminRole))
            {
                return EventRoleHelper.EslAdminRole;
            }

            if (User.IsInRole(EventRoleHelper.TournamentAdminRole))
            {
                return EventRoleHelper.TournamentAdminRole;
            }

            return null;
        }

        private string? GetLoggedInRoleBadgeClass(string? roleLabel)
        {
            return roleLabel switch
            {
                EventRoleHelper.TournamentAdminRole => "admin-role-badge admin-role-badge--tournament",
                EventRoleHelper.SuperAdminRole => "admin-role-badge admin-role-badge--super",
                EventRoleHelper.BlastAdminRole => "admin-role-badge admin-role-badge--blast",
                EventRoleHelper.EslAdminRole => "admin-role-badge admin-role-badge--esl",
                null => null,
                _ => "admin-role-badge"
            };
        }
    }
}
