using System;
using GameStore.Web.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GameStore.Web.Services;

public class UserCookieService : IUserCookieService
{
    private const string UserCookieName = "_userId";
    private readonly CookieOptions _cookieOptions = new() { Expires = DateTimeOffset.UtcNow.AddYears(2) };

    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserCookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private IRequestCookieCollection RequestCookies => _httpContextAccessor.HttpContext.Request.Cookies;

    private IResponseCookies ResponseCookies => _httpContextAccessor.HttpContext.Response.Cookies;

    public bool IsCookiesContainsUserId()
    {
        return RequestCookies.ContainsKey(UserCookieName);
    }

    public Guid GetCookiesUserId()
    {
        RequestCookies.TryGetValue(UserCookieName, out var userId);

        return Guid.Parse(userId);
    }

    public void AppendUserId(Guid userId)
    {
        ResponseCookies.Append(UserCookieName, userId.ToString(), _cookieOptions);
    }

    public void RemoveUserId()
    {
        ResponseCookies.Delete(UserCookieName);
    }
}