using cs2_esports.Models;

namespace cs2_esports.Repositories.Interfaces;

public interface IMatchRepository
{
    IReadOnlyList<Match> GetAll();
    Match? GetById(int id);
    void Add(Match match);
    void Update(Match match);
    void Delete(int id);
}