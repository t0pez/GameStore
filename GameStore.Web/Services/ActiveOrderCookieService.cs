using System;
using GameStore.Web.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GameStore.Web.Services;

public class ActiveOrderCookieService : IActiveOrderCookieService
{
    private const string ActiveOrderCookieName = "_activeOrder";

    public bool IsCookieContainsActiveOrder(IRequestCookieCollection cookies)
    {
        return cookies.ContainsKey(ActiveOrderCookieName);
    }

    public bool TryGetActiveOrderId(IRequestCookieCollection cookies, out Guid orderId)
    {
        var result = cookies.TryGetValue(ActiveOrderCookieName, out var cookieValue);
        orderId = Guid.Parse(cookieValue);

        return result;
    }

    public void AppendActiveOrder(IResponseCookies cookies, Guid orderId)
    {
        var activeOrder = orderId.ToString();
        
        cookies.Append(ActiveOrderCookieName, activeOrder);
    }

    public void RemoveActiveOrder(IResponseCookies cookies)
    {
        cookies.Delete(ActiveOrderCookieName);
    }
}