using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cs2_esports.Controllers;

public class TeamsController : Controller
{
    private readonly ITeamRepository _teamRepository;

    public TeamsController(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public IActionResult Index()
    {
        var teams = _teamRepository.GetAll();
        return View(teams);
    }

    public IActionResult Details(int id)
    {
        var team = _teamRepository.GetById(id);
        if (team is null)
        {
            return NotFound();
        }

        return View(team);
    }
}