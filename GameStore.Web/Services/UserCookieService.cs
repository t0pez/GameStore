using System;
using GameStore.Web.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GameStore.Web.Services;

public class UserCookieService : IUserCookieService
{
    private const string UserCookieName = "_userId";
    private readonly CookieOptions _cookieOptions = new() { Expires = DateTimeOffset.UtcNow.AddYears(2) };
    
    public bool IsCookiesContainsUserId(IRequestCookieCollection cookies)
    {
        return cookies.ContainsKey(UserCookieName);
    }

    public bool TryGetCookiesUserId(IRequestCookieCollection cookies, out string userId)
    {
        return cookies.TryGetValue(UserCookieName, out userId);
    }

    public void AppendUserId(IResponseCookies cookies, string userId)
    {
        cookies.Append(UserCookieName, userId, _cookieOptions);
    }
}