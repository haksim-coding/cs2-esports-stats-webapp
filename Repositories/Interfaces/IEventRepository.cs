using cs2_esports.Models;

namespace cs2_esports.Repositories.Interfaces;

public interface IEventRepository
{
    IReadOnlyList<Event> GetAll();
    Event? GetById(int id);
}
