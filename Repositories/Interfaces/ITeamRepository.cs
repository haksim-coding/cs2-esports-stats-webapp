using cs2_esports.Models;

namespace cs2_esports.Repositories.Interfaces;

public interface ITeamRepository
{
    IReadOnlyList<Team> GetAll();
    Team? GetById(int id);
    IReadOnlyList<Team> GetByIds(IEnumerable<int> ids);
    IReadOnlyList<Team> SearchByNameOrTag(string query, int take = 10);
    void Add(Team team);
    void Update(Team team);
    void Delete(int id);
}