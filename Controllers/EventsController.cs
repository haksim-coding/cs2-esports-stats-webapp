using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cs2_esports.Controllers;

public class EventsController : Controller
{
    private readonly IEventRepository _eventRepository;

    public EventsController(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
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
}