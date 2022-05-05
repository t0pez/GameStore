using System;
using GameStore.Web.Interfaces;
using GameStore.Web.Models.Baskets;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace GameStore.Web.Services;

public class BasketCookieService : IBasketCookieService
{
    private const string BasketCookieName = "_basket";
    private readonly CookieOptions _basketCookieOptions = new() { Expires = DateTimeOffset.UtcNow.AddDays(7) };

    public BasketCookieModel GetBasketFromCookie(IRequestCookieCollection cookies)
    {
        var result = cookies.TryGetValue(BasketCookieName, out var basketJson)
            ? JsonConvert.DeserializeObject<BasketCookieModel>(basketJson)
            : new BasketCookieModel();

        return result;
    }

    public void AppendBasketCookie(IResponseCookies cookies, BasketCookieModel basket)
    {
        var basketJson = JsonConvert.SerializeObject(basket);
        
        cookies.Append(BasketCookieName, basketJson, _basketCookieOptions);
    }
}