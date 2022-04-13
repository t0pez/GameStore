using System;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Baskets;
using GameStore.Web.Models.Basket;
using GameStore.Web.ViewModels.Basket;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GameStore.Web.Controllers;

public class BasketController : Controller
{
    private const string BasketCookieName = "_basket";
    private static readonly CookieOptions BasketCookieOptions = new() { Expires = DateTimeOffset.UtcNow.AddDays(7) };
    private readonly IBasketService _basketService;
    private readonly IMapper _mapper;

    public BasketController(IBasketService basketService, IMapper mapper)
    {
        _basketService = basketService;
        _mapper = mapper;
    }

    [HttpGet("basket/")]
    public async Task<ActionResult> GetCurrentBasketAsync()
    {
        var basketCookieModel = HttpContext.Request.Cookies.TryGetValue(BasketCookieName, out var basketJson)
            ? JsonConvert.DeserializeObject<BasketCookieModel>(basketJson)
            : new BasketCookieModel();

        var basket = _mapper.Map<Basket>(basketCookieModel);
        await _basketService.FillWithDetailsAsync(basket);

        var basketViewModel = _mapper.Map<BasketViewModel>(basket);

        return View(basketViewModel);
    }
    
    [HttpPost("games/buy")]
    public async Task<ActionResult> AddToBasketAsync(Guid gameId, int quantity)
    {
        var basketCookieModel = HttpContext.Request.Cookies.TryGetValue(BasketCookieName, out var basketJson)
            ? JsonConvert.DeserializeObject<BasketCookieModel>(basketJson)
            : new BasketCookieModel();

        var basket = _mapper.Map<Basket>(basketCookieModel);
        _basketService.AddToBasket(basket, gameId, quantity);
        
        basketCookieModel = _mapper.Map<BasketCookieModel>(basket);
        basketJson = JsonConvert.SerializeObject(basketCookieModel);
        
        HttpContext.Response.Cookies.Append(BasketCookieName, basketJson, BasketCookieOptions);

        return RedirectToAction("GetAll", "Games");
    }
}