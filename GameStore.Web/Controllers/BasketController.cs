using System;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Web.ViewModels.Baskets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GameStore.Web.Controllers;

public class BasketController : Controller
{
    private const string BasketCookieName = "_basket"; 
    
    [HttpGet("basket/")]
    public async Task<ActionResult> GetCurrentBasketAsync()
    {
        var basket = HttpContext.Request.Cookies.TryGetValue(BasketCookieName, out var basketJson)
            ? JsonConvert.DeserializeObject<BasketViewModel>(basketJson)
            : new BasketViewModel();

        return View(basket);
    }
    
    [HttpPost("games/buy")]
    public async Task<ActionResult> AddToBasketAsync(Guid gameId, int quantity)
    {
        BasketViewModel basket;
        var itemViewModel = new BasketItemViewModel
        {
            GameId = gameId,
            Quantity = quantity
        };
        
        if (HttpContext.Request.Cookies.TryGetValue(BasketCookieName, out var basketJson))
        {
            basket = JsonConvert.DeserializeObject<BasketViewModel>(basketJson);

            if (basket.Items.Any(model => model.GameId == itemViewModel.GameId))
            {
                basket.Items.First(model => model.GameId == itemViewModel.GameId).Quantity += itemViewModel.Quantity;
            }
            else
            {
                basket.Items.Add(itemViewModel);                
            }
        }
        else
        {
            basket = new BasketViewModel();
            basket.Items.Add(itemViewModel);
        }
        
        basketJson = JsonConvert.SerializeObject(basket);
        HttpContext.Response.Cookies.Append(BasketCookieName, basketJson, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return RedirectToAction("GetAll", "Games");
    }
}