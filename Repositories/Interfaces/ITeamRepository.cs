using cs2_esports.Models;

namespace cs2_esports.Repositories.Interfaces;

public interface ITeamRepository
{
    IReadOnlyList<Team> GetAll();
    Team? GetById(int id);
}