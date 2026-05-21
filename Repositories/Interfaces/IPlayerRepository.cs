using cs2_esports.Models;

namespace cs2_esports.Repositories.Interfaces;

public interface IPlayerRepository
{
    IReadOnlyList<Player> GetAllAlphabetical();
    Player? GetById(int id);
    IReadOnlyList<Player> SearchAvailableByNickname(string query, int? currentTeamId = null, int take = 10);
    IReadOnlyList<Player> GetByIds(IEnumerable<int> ids);
    void Add(Player player);
    void Update(Player player);
    void Delete(int id);
}