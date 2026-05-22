using cs2_esports.Data;
using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace cs2_esports.Repositories.Ef;

public class EfForumRepository : IForumRepository
{
    private readonly Cs2ScopeDbContext _context;

    public EfForumRepository(Cs2ScopeDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<Forum> GetAll()
    {
        return _context.Forums
            .Include(forum => forum.Author)
            .Include(forum => forum.Event)
            .OrderByDescending(forum => forum.IsPinned)
            .ThenByDescending(forum => forum.LastUpdatedAtUtc)
            .ToList();
    }

    public Forum? GetById(int id)
    {
        return _context.Forums
            .Include(forum => forum.Author)
            .Include(forum => forum.Event)
            .FirstOrDefault(forum => forum.Id == id);
    }

    public IReadOnlyList<ForumComment> GetCommentsByForumId(int forumId)
    {
        return _context.ForumComments
            .Include(comment => comment.Author)
            .Where(comment => comment.ForumId == forumId)
            .OrderBy(comment => comment.CreatedAtUtc)
            .ToList();
    }

    public IReadOnlyList<ForumUser> GetForumUsers()
    {
        return _context.ForumUsers
            .OrderBy(user => user.DisplayName)
            .ToList();
    }

    public ForumUser? GetForumUserById(int id)
    {
        return _context.ForumUsers.FirstOrDefault(user => user.Id == id);
    }

    public ForumUser? GetForumUserByUsernameOrEmail(string usernameOrEmail)
    {
        var normalized = usernameOrEmail.Trim().ToLower();
        return _context.ForumUsers.FirstOrDefault(user =>
            user.Username.ToLower() == normalized ||
            user.Email.ToLower() == normalized);
    }

    public IReadOnlyList<Event> GetTournaments()
    {
        return _context.Tournaments
            .OrderByDescending(tournament => tournament.StartDateUtc)
            .ToList();
    }

    public ForumUser? RegisterForumUser(ForumRegisterInputModel input)
    {
        var username = input.Username.Trim().ToLower();
        var email = input.Email.Trim().ToLower();

        if (_context.ForumUsers.Any(user => user.Username.ToLower() == username) ||
            _context.ForumUsers.Any(user => user.Email.ToLower() == email))
        {
            return null;
        }

        var user = new ForumUser
        {
            Username = input.Username.Trim(),
            DisplayName = input.DisplayName.Trim(),
            Email = input.Email.Trim(),
            CountryCode = input.CountryCode.Trim().ToUpperInvariant(),
            RegisteredAtUtc = DateTime.UtcNow,
            LastActiveAtUtc = DateTime.UtcNow,
            IsPremiumMember = false,
            Password = input.Password
        };

        _context.ForumUsers.Add(user);
        _context.SaveChanges();
        return user;
    }

    public Forum? Create(ForumCreateInputModel input)
    {
        var author = _context.ForumUsers.FirstOrDefault(user => user.Id == input.AuthorId);
        if (author is null)
        {
            return null;
        }

        Event? tournament = null;
        if (input.TournamentId.HasValue)
        {
            tournament = _context.Tournaments.FirstOrDefault(eventItem => eventItem.Id == input.TournamentId.Value);
            if (tournament is null)
            {
                return null;
            }
        }

        var now = DateTime.UtcNow;
        var forum = new Forum
        {
            Title = input.Title.Trim(),
            Content = input.Content.Trim(),
            Category = input.Category,
            CreatedAtUtc = now,
            LastUpdatedAtUtc = now,
            ViewCount = 0,
            IsPinned = false,
            IsLocked = false,
            Author = author,
            Event = tournament
        };

        _context.Forums.Add(forum);
        _context.SaveChanges();
        return forum;
    }

    public ForumComment? AddComment(ForumCommentInputModel input)
    {
        var forum = _context.Forums.FirstOrDefault(item => item.Id == input.ForumId);
        if (forum is null || forum.IsLocked)
        {
            return null;
        }

        var author = _context.ForumUsers.FirstOrDefault(user => user.Id == input.AuthorId);
        if (author is null)
        {
            return null;
        }

        var comment = new ForumComment
        {
            Forum = forum,
            Author = author,
            Content = input.Content.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            IsEdited = false
        };

        _context.ForumComments.Add(comment);
        forum.LastUpdatedAtUtc = comment.CreatedAtUtc;
        _context.SaveChanges();
        return comment;
    }

    public IReadOnlyList<Team> GetFavoriteTeams(int forumUserId)
    {
        return _context.ForumUsers
            .Where(user => user.Id == forumUserId)
            .SelectMany(user => user.FavoriteTeams)
            .Include(team => team.Players)
            .Include(team => team.Tournaments)
            .OrderBy(team => team.WorldRanking)
            .ToList();
    }

    public IReadOnlyList<Player> GetFavoritePlayers(int forumUserId)
    {
        return _context.ForumUsers
            .Where(user => user.Id == forumUserId)
            .SelectMany(user => user.FavoritePlayers)
            .Include(player => player.Team)
            .OrderBy(player => player.Nickname)
            .ThenBy(player => player.FullName)
            .ToList();
    }

    public bool UpdateForumUserProfile(int forumUserId, ForumUserEditProfileInputModel input)
    {
        var user = _context.ForumUsers.FirstOrDefault(item => item.Id == forumUserId);
        if (user is null)
        {
            return false;
        }

        var normalizedUsername = input.Username.Trim().ToLowerInvariant();
        var usernameExists = _context.ForumUsers.Any(item => item.Id != forumUserId && item.Username.ToLower() == normalizedUsername);
        if (usernameExists)
        {
            return false;
        }

        user.Username = input.Username.Trim();
        user.Bio = input.Bio.Trim();
        _context.SaveChanges();
        return true;
    }

    public bool DeleteForumUser(int forumUserId)
    {
        var user = _context.ForumUsers
            .Include(item => item.FavoriteTeams)
            .Include(item => item.FavoritePlayers)
            .FirstOrDefault(item => item.Id == forumUserId);

        if (user is null)
        {
            return false;
        }

        if (_context.Forums.Any(item => item.AuthorId == forumUserId) || _context.ForumComments.Any(item => item.AuthorId == forumUserId))
        {
            return false;
        }

        user.FavoriteTeams.Clear();
        user.FavoritePlayers.Clear();
        _context.ForumUsers.Remove(user);
        _context.SaveChanges();
        return true;
    }

    public bool ToggleFavoriteTeam(int forumUserId, int teamId)
    {
        var user = _context.ForumUsers
            .Include(item => item.FavoriteTeams)
            .FirstOrDefault(item => item.Id == forumUserId);

        var team = _context.Teams.FirstOrDefault(item => item.Id == teamId);
        if (user is null || team is null)
        {
            return false;
        }

        if (user.FavoriteTeams.Any(item => item.Id == teamId))
        {
            user.FavoriteTeams.Remove(team);
        }
        else
        {
            user.FavoriteTeams.Add(team);
        }

        _context.SaveChanges();
        return true;
    }

    public bool ToggleFavoritePlayer(int forumUserId, int playerId)
    {
        var user = _context.ForumUsers
            .Include(item => item.FavoritePlayers)
            .FirstOrDefault(item => item.Id == forumUserId);

        var player = _context.Players.FirstOrDefault(item => item.Id == playerId);
        if (user is null || player is null)
        {
            return false;
        }

        if (user.FavoritePlayers.Any(item => item.Id == playerId))
        {
            user.FavoritePlayers.Remove(player);
        }
        else
        {
            user.FavoritePlayers.Add(player);
        }

        _context.SaveChanges();
        return true;
    }
}