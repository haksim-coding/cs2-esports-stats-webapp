using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cs2_esports.Controllers;

public class ForumsController : Controller
{
    private const string ForumUserSessionKey = "ForumUserId";
    private readonly IForumRepository _forumRepository;

    public ForumsController(IForumRepository forumRepository)
    {
        _forumRepository = forumRepository;
    }

    public IActionResult Index()
    {
        var forums = _forumRepository.GetAll();
        return View(forums);
    }

    public IActionResult Details(int id)
    {
        var detailsViewModel = BuildDetailsViewModel(id);
        if (detailsViewModel is null)
        {
            return NotFound();
        }

        return View(detailsViewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (GetCurrentForumUser() is null)
        {
            return View(BuildCreatePageViewModel(new ForumCreateInputModel()));
        }

        var viewModel = BuildCreatePageViewModel(new ForumCreateInputModel());
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ForumCreateInputModel input)
    {
        var currentUser = GetCurrentForumUser();
        if (currentUser is null)
        {
            ModelState.AddModelError(string.Empty, "You need to log in to create a forum thread.");
            var unauthenticatedViewModel = BuildCreatePageViewModel(input);
            return View(unauthenticatedViewModel);
        }

        input.AuthorId = currentUser.Id;

        if (!ModelState.IsValid)
        {
            var invalidViewModel = BuildCreatePageViewModel(input);
            return View(invalidViewModel);
        }

        var createdForum = _forumRepository.Create(input);
        if (createdForum is null)
        {
            ModelState.AddModelError(string.Empty, "Unable to create forum thread. Please verify your selected author/event.");
            var invalidViewModel = BuildCreatePageViewModel(input);
            return View(invalidViewModel);
        }

        return RedirectToAction(nameof(Details), new { id = createdForum.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Comment(ForumCommentInputModel input)
    {
        var currentUser = GetCurrentForumUser();
        if (currentUser is null)
        {
            ModelState.AddModelError(string.Empty, "You need to log in to comment on a thread.");
            var unauthenticatedDetailsViewModel = BuildDetailsViewModel(input.ForumId, input);
            if (unauthenticatedDetailsViewModel is null)
            {
                return NotFound();
            }

            return View(nameof(Details), unauthenticatedDetailsViewModel);
        }

        input.AuthorId = currentUser.Id;

        if (!ModelState.IsValid)
        {
            var invalidDetailsViewModel = BuildDetailsViewModel(input.ForumId, input);
            if (invalidDetailsViewModel is null)
            {
                return NotFound();
            }

            return View(nameof(Details), invalidDetailsViewModel);
        }

        var createdComment = _forumRepository.AddComment(input);
        if (createdComment is null)
        {
            ModelState.AddModelError(string.Empty, "Unable to post comment. Thread may be locked or author is invalid.");
            var invalidDetailsViewModel = BuildDetailsViewModel(input.ForumId, input);
            if (invalidDetailsViewModel is null)
            {
                return NotFound();
            }

            return View(nameof(Details), invalidDetailsViewModel);
        }

        return RedirectToAction(nameof(Details), new { id = input.ForumId });
    }

    private ForumCreatePageViewModel BuildCreatePageViewModel(ForumCreateInputModel input)
    {
        return new ForumCreatePageViewModel
        {
            Input = input,
            Events = _forumRepository.GetTournaments(),
            CurrentUser = GetCurrentForumUser()
        };
    }

    private ForumDetailsViewModel? BuildDetailsViewModel(int id, ForumCommentInputModel? commentInput = null)
    {
        var forum = _forumRepository.GetById(id);
        if (forum is null)
        {
            return null;
        }

        return new ForumDetailsViewModel
        {
            Forum = forum,
            Comments = _forumRepository.GetCommentsByForumId(id),
            CurrentUser = GetCurrentForumUser(),
            NewComment = commentInput ?? new ForumCommentInputModel { ForumId = id }
        };
    }

    private ForumUser? GetCurrentForumUser()
    {
        var userId = HttpContext.Session.GetInt32(ForumUserSessionKey);
        if (!userId.HasValue)
        {
            return null;
        }

        return _forumRepository.GetForumUserById(userId.Value);
    }
}