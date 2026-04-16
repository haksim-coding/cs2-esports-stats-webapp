using cs2_esports.Models;

namespace cs2_esports.Repositories.Interfaces;

public interface IPlayerRepository
{
    IReadOnlyList<Player> GetAllAlphabetical();
    Player? GetById(int id);
}