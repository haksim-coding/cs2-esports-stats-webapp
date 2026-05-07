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
            .OrderBy(tournament => tournament.StartDateUtc)
            .ToList();
    }

    public Event? GetById(int id)
    {
        return _context.Tournaments
            .Include(tournament => tournament.EventVenue)
            .Include(tournament => tournament.Teams)
            .Include(tournament => tournament.ForumThreads)
            .FirstOrDefault(tournament => tournament.Id == id);
    }
}