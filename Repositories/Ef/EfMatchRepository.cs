using cs2_esports.Data;
using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace cs2_esports.Repositories.Ef;

public class EfMatchRepository : IMatchRepository
{
    private readonly Cs2ScopeDbContext _context;

    public EfMatchRepository(Cs2ScopeDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<Match> GetAll()
    {
        return _context.Matches
            .Include(match => match.Event)
            .Include(match => match.TeamA)
            .Include(match => match.TeamB)
            .Include(match => match.Maps)
            .OrderBy(match => match.ScheduledAtUtc)
            .ToList();
    }

    public Match? GetById(int id)
    {
        return _context.Matches
            .Include(match => match.Event)
            .Include(match => match.TeamA)
            .Include(match => match.TeamB)
            .Include(match => match.Maps)
            .FirstOrDefault(match => match.Id == id);
    }

    public void Add(Match match)
    {
        _context.Matches.Add(match);
        _context.SaveChanges();
    }

    public void Update(Match match)
    {
        var existingMatch = _context.Matches
            .Include(existing => existing.Maps)
            .FirstOrDefault(existing => existing.Id == match.Id);
        if (existingMatch is null)
        {
            return;
        }

        _context.Entry(existingMatch).CurrentValues.SetValues(match);
        _context.MatchMaps.RemoveRange(existingMatch.Maps);
        existingMatch.Maps.Clear();

        foreach (var map in match.Maps.OrderBy(item => item.MapSequence))
        {
            existingMatch.Maps.Add(new MatchMap
            {
                MapSequence = map.MapSequence,
                Map = map.Map,
                TeamAScore = map.TeamAScore,
                TeamBScore = map.TeamBScore,
                WentToOvertime = map.WentToOvertime
            });
        }

        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var match = _context.Matches.FirstOrDefault(existing => existing.Id == id);
        if (match is null)
        {
            return;
        }

        _context.Matches.Remove(match);
        _context.SaveChanges();
    }
}