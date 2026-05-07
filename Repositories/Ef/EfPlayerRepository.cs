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
}