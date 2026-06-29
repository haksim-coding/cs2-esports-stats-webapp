using cs2_esports.Repositories.Interfaces;
using cs2_esports.Models;
using cs2_esports.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cs2_esports.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace cs2_esports.Controllers;

public class AuthController : Controller
{
    private static readonly HashSet<string> SupportedExternalProviders = new(StringComparer.Ordinal)
    {
        "Google"
    };

    private readonly IForumRepository _forumRepository;
    private readonly Cs2ScopeDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AuthController(IForumRepository forumRepository, Cs2ScopeDbContext dbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _forumRepository = forumRepository;
        _dbContext = dbContext;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginInputModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginInputModel input, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(input);
        }

        var identityUser = await _userManager.FindByNameAsync(input.Username)
            ?? await _userManager.FindByEmailAsync(input.Username);

        if (identityUser is not null && await _userManager.CheckPasswordAsync(identityUser, input.Password))
        {
            ClearAuthenticationSession();
            await _signInManager.SignInAsync(identityUser, isPersistent: false);

            if (identityUser.LegacyForumUserId.HasValue)
            {
                SetAuthenticatedForumUser(identityUser.LegacyForumUserId.Value);
                var forumUser = _forumRepository.GetForumUserById(identityUser.LegacyForumUserId.Value);
                if (forumUser is not null)
                {
                    forumUser.LastActiveAtUtc = DateTime.UtcNow;
                    _dbContext.SaveChanges();
                }
            }

            var legacyAdminUserId = identityUser.LegacyAdminUserId
                ?? _dbContext.AdminUsers
                    .Where(user => user.Username == identityUser.UserName)
                    .Select(user => (int?)user.Id)
                    .FirstOrDefault();

            if (legacyAdminUserId.HasValue)
            {
                SetAuthenticatedAdminUser(legacyAdminUserId.Value);
            }

            return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
        }

        var normalizedUsername = input.Username.Trim().ToLowerInvariant();
        var adminUser = _dbContext.AdminUsers.FirstOrDefault(user =>
            user.Username.ToLower() == normalizedUsername ||
            user.Email.ToLower() == normalizedUsername);

        if (adminUser is null || !string.Equals(adminUser.Password, input.Password, StringComparison.Ordinal))
        {
            ModelState.AddModelError(string.Empty, "Invalid login details.");
            ViewData["ReturnUrl"] = returnUrl;
            return View(input);
        }

        var identityAdminUser = await _userManager.FindByNameAsync(adminUser.Username)
            ?? await _userManager.FindByEmailAsync(adminUser.Email);

        if (identityAdminUser is null)
        {
            ModelState.AddModelError(string.Empty, "This admin account has not been migrated to Identity yet.");
            ViewData["ReturnUrl"] = returnUrl;
            return View(input);
        }

        await _signInManager.SignInAsync(identityAdminUser, isPersistent: false);
        SetAuthenticatedAdminUser(adminUser.Id);
        return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLogin(string provider, string? returnUrl = null)
    {
        var configuredProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();
        if (!SupportedExternalProviders.Contains(provider) ||
            !configuredProviders.Any(item => string.Equals(item.Name, provider, StringComparison.Ordinal)))
        {
            return RedirectToAction(GetAuthEntryAction(), new { returnUrl });
        }

        await _signInManager.SignOutAsync();
        ClearAuthenticationSession();

        var callbackUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, callbackUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        var destination = GetSafeReturnUrl(returnUrl);
        if (!string.IsNullOrWhiteSpace(remoteError))
        {
            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
        if (loginInfo is null || !SupportedExternalProviders.Contains(loginInfo.LoginProvider))
        {
            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        var linkedUser = await _userManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
        if (linkedUser is not null)
        {
            if (linkedUser.LegacyAdminUserId.HasValue || !linkedUser.LegacyForumUserId.HasValue)
            {
                return RedirectToAction(nameof(Login), new { returnUrl });
            }

            ClearAuthenticationSession();
            await _signInManager.SignInAsync(linkedUser, isPersistent: false, loginInfo.LoginProvider);
            SetAuthenticatedForumUser(linkedUser.LegacyForumUserId.Value);

            var forumUser = await _dbContext.ForumUsers.FindAsync(linkedUser.LegacyForumUserId.Value);
            if (forumUser is not null)
            {
                forumUser.LastActiveAtUtc = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            return LocalRedirect(destination);
        }

        var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email)?.Trim();
        if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
        {
            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        var existingIdentity = await _userManager.FindByEmailAsync(email);
        var normalizedEmail = email.ToLowerInvariant();
        var existingLegacyUser = await _dbContext.Set<cs2_esports.Models.User>()
            .AnyAsync(user => user.Email.ToLower() == normalizedEmail);
        if (existingIdentity is not null || existingLegacyUser)
        {
            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        var displayName = loginInfo.Principal.FindFirstValue(ClaimTypes.Name)?.Trim();
        if (string.IsNullOrWhiteSpace(displayName))
        {
            displayName = email.Split('@')[0];
        }
        displayName = displayName[..Math.Min(displayName.Length, 60)];

        var username = await CreateUniqueExternalUsernameAsync(displayName, email);
        var now = DateTime.UtcNow;
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        var forumAccount = new ForumUser
        {
            Username = username,
            DisplayName = displayName,
            Email = email,
            CountryCode = "UN",
            RegisteredAtUtc = now,
            LastActiveAtUtc = now,
            IsPremiumMember = false,
            Password = "[EXTERNAL_IDENTITY_ACCOUNT]"
        };
        _dbContext.ForumUsers.Add(forumAccount);
        await _dbContext.SaveChangesAsync();

        var identityUser = new AppUser
        {
            UserName = username,
            Email = email,
            DisplayName = displayName,
            CountryCode = "UN",
            RegisteredAtUtc = now,
            LegacyForumUserId = forumAccount.Id,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(identityUser);
        if (!createResult.Succeeded)
        {
            await transaction.RollbackAsync();
            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        var addLoginResult = await _userManager.AddLoginAsync(identityUser, loginInfo);
        if (!addLoginResult.Succeeded)
        {
            await transaction.RollbackAsync();
            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        await transaction.CommitAsync();
        await _signInManager.SignInAsync(identityUser, isPersistent: false, loginInfo.LoginProvider);
        SetAuthenticatedForumUser(forumAccount.Id);
        return LocalRedirect(destination);
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new ForumRegisterInputModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(ForumRegisterInputModel input, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(input);
        }

        if (await _userManager.FindByNameAsync(input.Username.Trim()) is not null ||
            await _userManager.FindByEmailAsync(input.Email.Trim()) is not null)
        {
            ModelState.AddModelError(string.Empty, "Username or email is already in use.");
            ViewData["ReturnUrl"] = returnUrl;
            return View(input);
        }

        var createdUser = _forumRepository.RegisterForumUser(input);
        if (createdUser is null)
        {
            ModelState.AddModelError(string.Empty, "Username or email is already in use.");
            ViewData["ReturnUrl"] = returnUrl;
            return View(input);
        }

        var identityUser = new AppUser
        {
            UserName = createdUser.Username,
            Email = createdUser.Email,
            DisplayName = createdUser.DisplayName,
            Bio = createdUser.Bio,
            CountryCode = createdUser.CountryCode,
            RegisteredAtUtc = createdUser.RegisteredAtUtc,
            IsSuspended = createdUser.IsSuspended,
            LegacyForumUserId = createdUser.Id,
            EmailConfirmed = true
        };
        var identityResult = await _userManager.CreateAsync(identityUser, input.Password);
        if (!identityResult.Succeeded)
        {
            _forumRepository.DeleteForumUser(createdUser.Id);
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View(input);
        }

        await _signInManager.SignInAsync(identityUser, isPersistent: false);
        SetAuthenticatedForumUser(createdUser.Id);
        return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        ClearAuthenticationSession();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Authorize]
    public IActionResult Profile()
    {
        var currentUser = GetCurrentForumUser();
        if (currentUser is null)
        {
            return RedirectToAction(nameof(Login), new { returnUrl = Url.Action(nameof(Profile)) });
        }

        var model = new ForumUserProfileViewModel
        {
            User = currentUser,
            FavoriteTeams = _forumRepository.GetFavoriteTeams(currentUser.Id),
            FavoritePlayers = _forumRepository.GetFavoritePlayers(currentUser.Id)
        };

        return View(model);
    }

    [HttpGet]
    [Authorize]
    public IActionResult EditProfile()
    {
        var currentUser = GetCurrentForumUser();
        if (currentUser is null)
        {
            return RedirectToAction(nameof(Login), new { returnUrl = Url.Action(nameof(EditProfile)) });
        }

        return View(new ForumUserEditProfileInputModel
        {
            Username = currentUser.Username,
            Bio = currentUser.Bio
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> EditProfile(ForumUserEditProfileInputModel input)
    {
        var currentUser = GetCurrentForumUser();
        if (currentUser is null)
        {
            return RedirectToAction(nameof(Login), new { returnUrl = Url.Action(nameof(EditProfile)) });
        }

        if (!ModelState.IsValid)
        {
            return View(input);
        }

        var identityUser = await _userManager.GetUserAsync(User);
        if (identityUser is null || identityUser.LegacyForumUserId != currentUser.Id)
        {
            return Forbid();
        }

        var requestedUsername = input.Username.Trim();
        var identityUserWithUsername = await _userManager.FindByNameAsync(requestedUsername);
        if (identityUserWithUsername is not null && identityUserWithUsername.Id != identityUser.Id)
        {
            ModelState.AddModelError(nameof(input.Username), "That username is already in use.");
            return View(input);
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        if (!_forumRepository.UpdateForumUserProfile(currentUser.Id, input))
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(nameof(input.Username), "That username is already in use.");
            return View(input);
        }

        identityUser.UserName = requestedUsername;
        identityUser.DisplayName = currentUser.DisplayName;
        identityUser.Bio = input.Bio.Trim();
        var updateResult = await _userManager.UpdateAsync(identityUser);
        if (!updateResult.Succeeded)
        {
            await transaction.RollbackAsync();
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(input);
        }

        await transaction.CommitAsync();
        await _signInManager.RefreshSignInAsync(identityUser);

        TempData["ProfileMessage"] = "Profile updated.";
        return RedirectToAction(nameof(Profile));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteAccount()
    {
        var currentUser = GetCurrentForumUser();
        if (currentUser is null)
        {
            return RedirectToAction(nameof(Login));
        }

        var identityUser = await _userManager.GetUserAsync(User);
        if (identityUser is null || identityUser.LegacyForumUserId != currentUser.Id)
        {
            return Forbid();
        }

        if (!_forumRepository.DeleteForumUser(currentUser.Id))
        {
            TempData["ProfileMessage"] = "Your account cannot be deleted while forum posts or comments still exist.";
            return RedirectToAction(nameof(Profile));
        }

        await _userManager.DeleteAsync(identityUser);
        await _signInManager.SignOutAsync();
        ClearAuthenticationSession();
        return RedirectToAction("Index", "Home");
    }

    private ForumUser? GetCurrentForumUser()
    {
        var userType = HttpContext.Session.GetString(AuthSessionKeys.UserType);
        if (!string.Equals(userType, AuthSessionKeys.ForumUserType, StringComparison.Ordinal))
        {
            return null;
        }

        var userId = HttpContext.Session.GetInt32(AuthSessionKeys.ForumUserId);
        return userId.HasValue ? _forumRepository.GetForumUserById(userId.Value) : null;
    }

    private void SetAuthenticatedForumUser(int userId)
    {
        ClearAuthenticationSession();
        HttpContext.Session.SetInt32(AuthSessionKeys.ForumUserId, userId);
        HttpContext.Session.SetString(AuthSessionKeys.UserType, AuthSessionKeys.ForumUserType);
    }

    private void SetAuthenticatedAdminUser(int userId)
    {
        ClearAuthenticationSession();
        HttpContext.Session.SetInt32(AuthSessionKeys.AdminUserId, userId);
        HttpContext.Session.SetString(AuthSessionKeys.UserType, AuthSessionKeys.AdminUserType);
    }

    private void ClearAuthenticationSession()
    {
        HttpContext.Session.Remove(AuthSessionKeys.ForumUserId);
        HttpContext.Session.Remove(AuthSessionKeys.AdminUserId);
        HttpContext.Session.Remove(AuthSessionKeys.UserType);
    }

    private string GetSafeReturnUrl(string? returnUrl)
    {
        return Url.IsLocalUrl(returnUrl) ? returnUrl! : Url.Action("Index", "Home")!;
    }

    private string GetAuthEntryAction()
    {
        var referer = Request.GetTypedHeaders().Referer;
        return string.Equals(referer?.AbsolutePath, Url.Action(nameof(Register)), StringComparison.OrdinalIgnoreCase)
            ? nameof(Register)
            : nameof(Login);
    }

    private async Task<string> CreateUniqueExternalUsernameAsync(string displayName, string email)
    {
        var source = string.IsNullOrWhiteSpace(displayName) ? email.Split('@')[0] : displayName;
        var cleaned = new string(source
            .Where(character => char.IsLetterOrDigit(character) || character is '-' or '_' or '.')
            .ToArray());
        if (cleaned.Length < 3)
        {
            cleaned = $"user{cleaned}";
        }
        cleaned = cleaned[..Math.Min(cleaned.Length, 32)];

        for (var suffix = 0; suffix < 10_000; suffix++)
        {
            var candidate = suffix == 0 ? cleaned : $"{cleaned}{suffix}";
            if (candidate.Length > 40)
            {
                candidate = candidate[..40];
            }

            if (await _userManager.FindByNameAsync(candidate) is null &&
                !await _dbContext.Set<cs2_esports.Models.User>()
                    .AnyAsync(user => user.Username.ToLower() == candidate.ToLower()))
            {
                return candidate;
            }
        }

        return $"user{Guid.NewGuid():N}";
    }
}
