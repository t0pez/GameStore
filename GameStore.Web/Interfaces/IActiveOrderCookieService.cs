using System;
using Microsoft.AspNetCore.Http;

namespace GameStore.Web.Interfaces;

public interface IActiveOrderCookieService
{
    public bool IsCookieContainsActiveOrder(IRequestCookieCollection cookies);
    public bool TryGetActiveOrderId(IRequestCookieCollection cookies, out Guid orderId);
    public void AppendActiveOrder(IResponseCookies cookies, Guid orderId);
    public void RemoveActiveOrder(IResponseCookies cookies);
}