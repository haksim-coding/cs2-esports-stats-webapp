using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;

namespace cs2_esports.Repositories.Mock;

public class TeamMockRepository : ITeamRepository
{
    private readonly InMemoryAppData _data;

    public TeamMockRepository(InMemoryAppData data)
    {
        _data = data;
    }

    public IReadOnlyList<Team> GetAll()
    {
        return _data.Teams
            .OrderBy(team => team.WorldRanking)
            .ToList();
    }

    public Team? GetById(int id)
    {
        return _data.Teams.FirstOrDefault(team => team.Id == id);
    }
}