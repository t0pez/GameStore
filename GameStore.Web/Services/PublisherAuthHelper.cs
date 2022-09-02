using System.Threading.Tasks;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Constants;
using GameStore.Core.Models.Server.Users;
using GameStore.Web.Interfaces;

namespace GameStore.Web.Services;

public class PublisherAuthHelper : IPublisherAuthHelper
{
    private readonly IUserCookieService _userCookieService;
    private readonly IUserService _userService;

    public PublisherAuthHelper(IUserCookieService userCookieService, IUserService userService)
    {
        _userCookieService = userCookieService;
        _userService = userService;
    }

    public async Task<bool> CanEditAsync(string publisherName)
    {
        var userId = _userCookieService.GetCookiesUserId();

        var user = await _userService.GetByIdAsync(userId)
                   ?? throw new ItemNotFoundException(typeof(User), userId);

        return IsUserNotPublisher(user) || IsUserPublisherOfSameName(user, publisherName);
    }

    private bool IsUserPublisherOfSameName(User user, string publisherName)
    {
        return user.Role == Roles.Publisher && user.PublisherName == publisherName;
    }

    private bool IsUserNotPublisher(User user)
    {
        return user.Role != Roles.Publisher;
    }
}