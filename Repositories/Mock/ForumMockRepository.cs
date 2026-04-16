using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;

namespace cs2_esports.Repositories.Mock;

public class ForumMockRepository : IForumRepository
{
    private readonly InMemoryAppData _data;

    public ForumMockRepository(InMemoryAppData data)
    {
        _data = data;
    }

    public IReadOnlyList<Forum> GetAll()
    {
        return _data.Forums
            .OrderByDescending(forum => forum.IsPinned)
            .ThenByDescending(forum => forum.LastUpdatedAtUtc)
            .ToList();
    }

    public Forum? GetById(int id)
    {
        return _data.Forums.FirstOrDefault(forum => forum.Id == id);
    }

    public IReadOnlyList<ForumComment> GetCommentsByForumId(int forumId)
    {
        return _data.ForumComments
            .Where(comment => comment.ForumId == forumId)
            .OrderBy(comment => comment.CreatedAtUtc)
            .ToList();
    }

    public IReadOnlyList<ForumUser> GetForumUsers()
    {
        return _data.ForumUsers
            .OrderBy(user => user.DisplayName)
            .ToList();
    }

    public ForumUser? GetForumUserById(int id)
    {
        return _data.ForumUsers.FirstOrDefault(user => user.Id == id);
    }

    public ForumUser? GetForumUserByUsernameOrEmail(string usernameOrEmail)
    {
        return _data.ForumUsers.FirstOrDefault(user =>
            string.Equals(user.Username, usernameOrEmail, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(user.Email, usernameOrEmail, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<Event> GetTournaments()
    {
        return _data.Tournaments
            .OrderByDescending(tournament => tournament.StartDateUtc)
            .ToList();
    }

    public ForumUser? RegisterForumUser(ForumRegisterInputModel input)
    {
        var existingUsername = _data.ForumUsers.Any(user => string.Equals(user.Username, input.Username, StringComparison.OrdinalIgnoreCase));
        var existingEmail = _data.ForumUsers.Any(user => string.Equals(user.Email, input.Email, StringComparison.OrdinalIgnoreCase));
        if (existingUsername || existingEmail)
        {
            return null;
        }

        var nextId = _data.ForumUsers.Count == 0 ? 1 : _data.ForumUsers.Max(user => user.Id) + 1;
        var user = new ForumUser
        {
            Id = nextId,
            Username = input.Username.Trim(),
            DisplayName = input.DisplayName.Trim(),
            Email = input.Email.Trim(),
            CountryCode = input.CountryCode.Trim().ToUpperInvariant(),
            RegisteredAtUtc = DateTime.UtcNow,
            LastActiveAtUtc = DateTime.UtcNow,
            IsPremiumMember = false,
            Password = input.Password
        };

        _data.ForumUsers.Add(user);
        return user;
    }

    public Forum? Create(ForumCreateInputModel input)
    {
        var author = _data.ForumUsers.FirstOrDefault(user => user.Id == input.AuthorId);
        if (author is null)
        {
            return null;
        }

        Event? tournament = null;
        if (input.TournamentId.HasValue)
        {
            tournament = _data.Tournaments.FirstOrDefault(eventData => eventData.Id == input.TournamentId.Value);
            if (tournament is null)
            {
                return null;
            }
        }

        var now = DateTime.UtcNow;
        var nextId = _data.Forums.Count == 0 ? 1 : _data.Forums.Max(forum => forum.Id) + 1;

        var forum = new Forum
        {
            Id = nextId,
            Title = input.Title.Trim(),
            Content = input.Content.Trim(),
            Category = input.Category,
            CreatedAtUtc = now,
            LastUpdatedAtUtc = now,
            ViewCount = 0,
            IsPinned = false,
            IsLocked = false,
            AuthorId = author.Id,
            Author = author,
            TournamentId = tournament?.Id,
            Event = tournament
        };

        _data.Forums.Add(forum);
        author.Threads.Add(forum);
        tournament?.ForumThreads.Add(forum);

        return forum;
    }

    public ForumComment? AddComment(ForumCommentInputModel input)
    {
        var forum = _data.Forums.FirstOrDefault(item => item.Id == input.ForumId);
        if (forum is null || forum.IsLocked)
        {
            return null;
        }

        var author = _data.ForumUsers.FirstOrDefault(user => user.Id == input.AuthorId);
        if (author is null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var nextId = _data.ForumComments.Count == 0 ? 1 : _data.ForumComments.Max(comment => comment.Id) + 1;
        var comment = new ForumComment
        {
            Id = nextId,
            ForumId = forum.Id,
            Forum = forum,
            AuthorId = author.Id,
            Author = author,
            Content = input.Content.Trim(),
            CreatedAtUtc = now,
            IsEdited = false
        };

        _data.ForumComments.Add(comment);
        forum.Comments.Add(comment);
        forum.LastUpdatedAtUtc = now;
        author.Comments.Add(comment);

        return comment;
    }
}