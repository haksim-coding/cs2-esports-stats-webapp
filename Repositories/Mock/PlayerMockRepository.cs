using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;

namespace cs2_esports.Repositories.Mock;

public class PlayerMockRepository : IPlayerRepository
{
    private readonly InMemoryAppData _data;

    public PlayerMockRepository(InMemoryAppData data)
    {
        _data = data;
    }

    public IReadOnlyList<Player> GetAllAlphabetical()
    {
        return _data.Players
            .OrderBy(player => player.Nickname)
            .ThenBy(player => player.FullName)
            .ToList();
    }

    public Player? GetById(int id)
    {
        return _data.Players.FirstOrDefault(player => player.Id == id);
    }
}