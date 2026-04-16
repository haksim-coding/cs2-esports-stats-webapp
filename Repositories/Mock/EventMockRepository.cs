using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;

namespace cs2_esports.Repositories.Mock;

public class EventMockRepository : IEventRepository
{
    private readonly InMemoryAppData _data;

    public EventMockRepository(InMemoryAppData data)
    {
        _data = data;
    }

    public IReadOnlyList<Event> GetAll()
    {
        return _data.Tournaments
            .OrderBy(tournament => tournament.StartDateUtc)
            .ToList();
    }

    public Event? GetById(int id)
    {
        return _data.Tournaments.FirstOrDefault(tournament => tournament.Id == id);
    }
}
