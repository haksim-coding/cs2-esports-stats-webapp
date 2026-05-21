using cs2_esports.Data;
using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace cs2_esports.Repositories.Ef;

public class EfPlayerRepository : IPlayerRepository
{
    private readonly Cs2ScopeDbContext _context;

    public EfPlayerRepository(Cs2ScopeDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<Player> GetAllAlphabetical()
    {
        return _context.Players
            .Include(player => player.Team)
            .OrderBy(player => player.Nickname)
            .ThenBy(player => player.FullName)
            .ToList();
    }

    public Player? GetById(int id)
    {
        return _context.Players
            .Include(player => player.Team)
            .FirstOrDefault(player => player.Id == id);
    }

    public IReadOnlyList<Player> SearchAvailableByNickname(string query, int? currentTeamId = null, int take = 10)
    {
        var normalizedQuery = query.Trim();

        var queryable = _context.Players
            .AsNoTracking()
            .Where(player => player.Nickname.Contains(normalizedQuery));

        if (currentTeamId.HasValue)
        {
            queryable = queryable.Where(player => player.TeamId == null || player.TeamId == currentTeamId.Value);
        }
        else
        {
            queryable = queryable.Where(player => player.TeamId == null);
        }

        return queryable
            .OrderBy(player => player.Nickname)
            .ThenBy(player => player.FullName)
            .Take(take)
            .ToList();
    }

    public IReadOnlyList<Player> GetByIds(IEnumerable<int> ids)
    {
        var selectedIds = ids.Distinct().ToArray();
        if (selectedIds.Length == 0)
        {
            return [];
        }

        return _context.Players
            .Where(player => selectedIds.Contains(player.Id))
            .ToList();
    }

    public void Add(Player player)
    {
        _context.Players.Add(player);
        _context.SaveChanges();
    }

    public void Update(Player player)
    {
        var existingPlayer = _context.Players.FirstOrDefault(existing => existing.Id == player.Id);
        if (existingPlayer is null)
        {
            return;
        }

        _context.Entry(existingPlayer).CurrentValues.SetValues(player);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var player = _context.Players.FirstOrDefault(existing => existing.Id == id);
        if (player is null)
        {
            return;
        }

        _context.Players.Remove(player);
        _context.SaveChanges();
    }
}