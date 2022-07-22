using Microsoft.AspNetCore.Http;

namespace GameStore.Web.Interfaces;

public interface IUserCookieService
{
    public bool IsCookiesContainsUserId(IRequestCookieCollection cookies);
    public bool TryGetCookiesUserId(IRequestCookieCollection cookies, out string userId);
    public void AppendUserId(IResponseCookies cookies, string userId);
}