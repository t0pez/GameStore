using System;
using System.Threading.Tasks;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Constants;
using GameStore.Core.Models.Server.Users;
using GameStore.Web.Interfaces;

namespace GameStore.Web.Services;

public class UserAuthHelper : IUserAuthHelper
{
    private readonly IUserService _userService;
    private readonly IUserCookieService _userCookieService;

    public UserAuthHelper(IUserService userService, IUserCookieService userCookieService)
    {
        _userService = userService;
        _userCookieService = userCookieService;
    }

    public async Task<bool> CanViewAndEditAsync(Guid idOfUserToEdit)
    {
        var userId = _userCookieService.GetCookiesUserId();

        var user = await _userService.GetByIdAsync(userId)
                   ?? throw new ItemNotFoundException(typeof(User), userId);

        return IsUserAdministrator(user) || (IsUserNotAdministrator(user) && IsUserOfSameId(user, idOfUserToEdit));
    }

    public async Task<bool> CanDeleteAsync(Guid idOfUserToDelete)
    {
        var userId = _userCookieService.GetCookiesUserId();

        var user = await _userService.GetByIdAsync(userId)
                   ?? throw new ItemNotFoundException(typeof(User), userId);

        return IsUserAdministrator(user) &&
               IsUserOfSameId(user, idOfUserToDelete) == false;
    }

    public async Task<bool> IsSameUserAsync(Guid idOfUserToCheck)
    {
        var userId = _userCookieService.GetCookiesUserId();

        var user = await _userService.GetByIdAsync(userId)
                   ?? throw new ItemNotFoundException(typeof(User), userId);

        return IsUserOfSameId(user, idOfUserToCheck);
    }

    private static bool IsUserAdministrator(User user)
    {
        return user.Role == Roles.Administrator;
    }

    private static bool IsUserNotAdministrator(User user)
    {
        return user.Role != Roles.Administrator;
    }

    private static bool IsUserOfSameId(User user, Guid id)
    {
        return user.Id == id;
    }
}