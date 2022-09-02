using System.Threading.Tasks;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Constants;
using GameStore.Core.Models.Server.Users;
using GameStore.Web.Extensions;
using GameStore.Web.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace GameStore.Web.Middlewares;

public class UserRoleMiddleware : IMiddleware
{
    private readonly IUserService _userService;
    private readonly IUserCookieService _userCookieService;
    private HttpContext _context;

    public UserRoleMiddleware(IUserService userService, IUserCookieService userCookieService)
    {
        _userService = userService;
        _userCookieService = userCookieService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.Identity.IsAuthenticated == false)
        {
            await next(context);

            return;
        }

        _context = context;

        var userId = _userCookieService.GetCookiesUserId();

        var user = await _userService.GetByIdAsync(userId);

        if (IsUserUserNameCorrect(user) == false ||
            IsUserRoleCorrect(user) == false)
        {
            await context.SignOutAsync();
        }

        await next(context);
    }

    private bool IsUserUserNameCorrect(User user)
    {
        var isUserNameClaimExists = _context.User.TryGetUserName(out var userName);

        if (isUserNameClaimExists)
        {
            return user.UserName == userName;
        }

        return false;
    }

    private bool IsUserRoleCorrect(User user)
    {
        var isRoleClaimExists = _context.User.TryGetRole(out var role);

        if (isRoleClaimExists == false)
        {
            return false;
        }

        if (role == Roles.Publisher)
        {
            return IsPublisherNameCorrect(user);
        }

        return user.Role == role;
    }

    private bool IsPublisherNameCorrect(User user)
    {
        var isPublisherNameExists = _context.User.TryGetPublisherName(out var publisherName);

        if (isPublisherNameExists == false)
        {
            return false;
        }

        if (string.IsNullOrEmpty(publisherName))
        {
            return false;
        }

        return user.PublisherName == publisherName;
    }
}