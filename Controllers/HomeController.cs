using cs2_esports.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using cs2_esports.Repositories.Interfaces;

namespace cs2_esports.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventRepository _eventRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IForumRepository _forumRepository;

        public HomeController(
            IEventRepository eventRepository,
            IPlayerRepository playerRepository,
            ITeamRepository teamRepository,
            IForumRepository forumRepository)
        {
            _eventRepository = eventRepository;
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
            _forumRepository = forumRepository;
        }

        public IActionResult Index()
        {
            var loggedInUserId = HttpContext.Session.GetInt32("ForumUserId");
            var loggedInUser = loggedInUserId.HasValue ? _forumRepository.GetForumUserById(loggedInUserId.Value) : null;
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

                LoggedInUser = loggedInUser
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
    }
}
