using cs2_esports.Data;
using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace cs2_esports.Repositories.Ef;

public class EfEventRepository : IEventRepository
{
    private readonly Cs2ScopeDbContext _context;

    public EfEventRepository(Cs2ScopeDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<Event> GetAll()
    {
        return _context.Tournaments
            .Include(tournament => tournament.EventVenue)
            .Include(tournament => tournament.Teams)
            .OrderBy(tournament => tournament.StartDateUtc)
            .ToList();
    }

    public Event? GetById(int id)
    {
        return _context.Tournaments
            .Include(tournament => tournament.EventVenue)
            .Include(tournament => tournament.Teams)
            .Include(tournament => tournament.Matches)
                .ThenInclude(match => match.TeamA)
            .Include(tournament => tournament.Matches)
                .ThenInclude(match => match.TeamB)
            .Include(tournament => tournament.Matches)
                .ThenInclude(match => match.Maps)
            .Include(tournament => tournament.ForumThreads)
            .FirstOrDefault(tournament => tournament.Id == id);
    }

    public void Add(Event eventItem)
    {
        _context.Tournaments.Add(eventItem);
        _context.SaveChanges();
    }

    public void Update(Event eventItem)
    {
        var existingEvent = _context.Tournaments
            .Include(tournament => tournament.Teams)
            .FirstOrDefault(tournament => tournament.Id == eventItem.Id);

        if (existingEvent is null)
        {
            return;
        }

        _context.Entry(existingEvent).CurrentValues.SetValues(eventItem);
        var selectedTeams = eventItem.Teams.ToList();
        existingEvent.Teams.Clear();
        foreach (var team in selectedTeams)
        {
            existingEvent.Teams.Add(team);
        }

        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var eventItem = _context.Tournaments
            .Include(tournament => tournament.Teams)
            .Include(tournament => tournament.Matches)
            .Include(tournament => tournament.ForumThreads)
            .FirstOrDefault(tournament => tournament.Id == id);

        if (eventItem is null)
        {
            return;
        }

        eventItem.Teams.Clear();
        _context.Matches.RemoveRange(eventItem.Matches);
        foreach (var forumThread in eventItem.ForumThreads)
        {
            forumThread.Event = null;
            forumThread.TournamentId = null;
        }

        _context.Tournaments.Remove(eventItem);
        _context.SaveChanges();
    }
}
