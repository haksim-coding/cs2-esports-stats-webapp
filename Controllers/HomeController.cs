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
                CanCreateForumPost = loggedInUser is ForumUser
            };

            return View(model);
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
                var userId = HttpContext.Session.GetInt32(AuthSessionKeys.AdminUserId);
                return userId.HasValue ? _dbContext.AdminUsers.FirstOrDefault(user => user.Id == userId.Value) : null;
            }

            return null;
        }
    }
}
