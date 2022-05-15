using GameStore.Web.Models.Baskets;
using Microsoft.AspNetCore.Http;

namespace GameStore.Web.Interfaces;

public interface IBasketCookieService
{
    public BasketCookieModel GetBasketFromCookie(IRequestCookieCollection cookies);
    public void AppendBasketCookie(IResponseCookies cookies, BasketCookieModel basket);
}