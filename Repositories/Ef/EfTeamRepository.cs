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

    public IReadOnlyList<Team> SearchByNameOrTag(string query, int take = 10)
    {
        var normalizedQuery = query.Trim();

        return _context.Teams
            .AsNoTracking()
            .Where(team => team.Name.Contains(normalizedQuery) || team.Tag.Contains(normalizedQuery))
            .OrderBy(team => team.WorldRanking)
            .ThenBy(team => team.Name)
            .Take(take)
            .ToList();
    }

    public Team? GetById(int id)
    {
        return _context.Teams
            .Include(team => team.Players)
            .Include(team => team.Tournaments)
            .Include(team => team.HomeMatches)
            .Include(team => team.AwayMatches)
            .FirstOrDefault(team => team.Id == id);
    }

    public IReadOnlyList<Team> GetByIds(IEnumerable<int> ids)
    {
        var selectedIds = ids.Distinct().ToArray();
        if (selectedIds.Length == 0)
        {
            return [];
        }

        return _context.Teams
            .Where(team => selectedIds.Contains(team.Id))
            .ToList();
    }

    public void Add(Team team)
    {
        _context.Teams.Add(team);
        _context.SaveChanges();
    }

    public void Update(Team team)
    {
        var existingTeam = _context.Teams.FirstOrDefault(existing => existing.Id == team.Id);
        if (existingTeam is null)
        {
            return;
        }

        _context.Entry(existingTeam).CurrentValues.SetValues(team);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var team = _context.Teams.FirstOrDefault(existing => existing.Id == id);
        if (team is null)
        {
            return;
        }

        _context.Teams.Remove(team);
        _context.SaveChanges();
    }
}