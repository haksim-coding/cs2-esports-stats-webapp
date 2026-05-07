using cs2_esports.Data;
using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace cs2_esports.Repositories.Ef;

public class EfTeamRepository : ITeamRepository
{
    private readonly Cs2ScopeDbContext _context;

    public EfTeamRepository(Cs2ScopeDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<Team> GetAll()
    {
        return _context.Teams
            .Include(team => team.Players)
            .Include(team => team.Tournaments)
            .OrderBy(team => team.WorldRanking)
            .ToList();
    }

    public Team? GetById(int id)
    {
        return _context.Teams
            .Include(team => team.Players)
            .Include(team => team.Tournaments)
                .ThenInclude(tournament => tournament.EventVenue)
            .FirstOrDefault(team => team.Id == id);
    }
}