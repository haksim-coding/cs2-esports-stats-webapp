using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cs2_esports.Controllers;

public class PlayersController : Controller
{
    private readonly IPlayerRepository _playerRepository;

    public PlayersController(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public IActionResult Index()
    {
        var players = _playerRepository.GetAllAlphabetical();
        return View(players);
    }

    public IActionResult Details(int id)
    {
        var player = _playerRepository.GetById(id);
        if (player is null)
        {
            return NotFound();
        }

        return View(player);
    }
}