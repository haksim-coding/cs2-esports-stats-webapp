using cs2_esports.Models;

namespace cs2_esports.Repositories.Interfaces;

public interface IForumRepository
{
    IReadOnlyList<Forum> GetAll();
    Forum? GetById(int id);
    IReadOnlyList<ForumComment> GetCommentsByForumId(int forumId);
    IReadOnlyList<ForumUser> GetForumUsers();
    ForumUser? GetForumUserById(int id);
    ForumUser? GetForumUserByUsernameOrEmail(string usernameOrEmail);
    IReadOnlyList<Event> GetTournaments();
    ForumUser? RegisterForumUser(ForumRegisterInputModel input);
    Forum? Create(ForumCreateInputModel input);
    ForumComment? AddComment(ForumCommentInputModel input);
}