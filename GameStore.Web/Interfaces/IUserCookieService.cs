using System;

namespace GameStore.Web.Interfaces;

public interface IUserCookieService
{
    public bool IsCookiesContainsUserId();
    public Guid GetCookiesUserId();
    public void AppendUserId(Guid userId);
    public void RemoveUserId();
}