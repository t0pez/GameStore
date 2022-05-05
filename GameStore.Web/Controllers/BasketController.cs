using System;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Baskets;
using GameStore.Web.Interfaces;
using GameStore.Web.Models.Baskets;
using GameStore.Web.ViewModels.Baskets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

public class BasketController : Controller
{
    private readonly IBasketCookieService _basketCookieService;
    private readonly IBasketService _basketService;
    private readonly IMapper _mapper;

    public BasketController(IBasketCookieService basketCookieService, IBasketService basketService, IMapper mapper)
    {
        _basketService = basketService;
        _mapper = mapper;
        _basketCookieService = basketCookieService;
    }

    private IRequestCookieCollection RequestCookies => HttpContext.Request.Cookies;
    private IResponseCookies ResponseCookies => HttpContext.Response.Cookies;
    
    [HttpGet("basket")]
    public async Task<ActionResult> GetCurrentBasketAsync()
    {
        var basketCookieModel = _basketCookieService.GetBasketFromCookie(RequestCookies);

        var basket = _mapper.Map<Basket>(basketCookieModel);
        await _basketService.FillWithDetailsAsync(basket);

        var basketViewModel = _mapper.Map<BasketViewModel>(basket);
        
        UpdateCookie(basket);
        
        return View(basketViewModel);
    }
    
    [HttpPost("games/{gameKey}/buy")]
    public async Task<ActionResult> AddToBasketAsync(Guid gameId, int quantity)
    {
        var basketCookieModel = _basketCookieService.GetBasketFromCookie(RequestCookies);

        var basket = _mapper.Map<Basket>(basketCookieModel);
        _basketService.AddToBasket(basket, gameId, quantity);
        
        UpdateCookie(basket);

        return RedirectToAction("GetAll", "Games");
    }

    private void UpdateCookie(Basket basket)
    {
        var basketCookieModel = _mapper.Map<BasketCookieModel>(basket);
        _basketCookieService.AppendBasketCookie(ResponseCookies, basketCookieModel);
    }
}