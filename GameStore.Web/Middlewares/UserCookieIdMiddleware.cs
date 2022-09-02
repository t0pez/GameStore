using System.Threading.Tasks;
using GameStore.Core.Interfaces;
using GameStore.Web.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GameStore.Web.Middlewares;

public class UserCookieIdMiddleware : IMiddleware
{
    private readonly IUserCookieService _userCookieService;
    private readonly IUserService _userService;

    public UserCookieIdMiddleware(IUserService userService, IUserCookieService userCookieService)
    {
        _userService = userService;
        _userCookieService = userCookieService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (_userCookieService.IsCookiesContainsUserId() == false)
        {
            var customerId = await _userService.GenerateUniqueUserIdAsync();
            _userCookieService.AppendUserId(customerId);
        }

        await next(context);
    }
}