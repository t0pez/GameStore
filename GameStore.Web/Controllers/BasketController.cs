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
    private readonly IActiveOrderCookieService _activeOrderCookieService;
    private readonly IMapper _mapper;

    public BasketController(IBasketCookieService basketCookieService, IBasketService basketService,
                            IActiveOrderCookieService activeOrderCookieService, IMapper mapper)
    {
        _basketService = basketService;
        _mapper = mapper;
        _activeOrderCookieService = activeOrderCookieService;
        _basketCookieService = basketCookieService;
    }

    private IRequestCookieCollection RequestCookies => HttpContext.Request.Cookies;
    private IResponseCookies ResponseCookies => HttpContext.Response.Cookies;
    
    [HttpGet("basket")]
    public async Task<ActionResult> GetCurrentBasketAsync()
    {
        var basket = GetBasket();
        
        await _basketService.FillWithDetailsAsync(basket);

        var basketViewModel = _mapper.Map<BasketViewModel>(basket);
        
        UpdateCookie(basket);

        var hasActiveOrder = _activeOrderCookieService.IsCookieContainsActiveOrder(HttpContext.Request.Cookies);
        ViewData["HasActiveOrder"] = hasActiveOrder;
        
        return View(basketViewModel);
    }
    
    [HttpPost("games/{gameKey}/buy")]
    public async Task<ActionResult> AddToBasketAsync(Guid gameId, int quantity)
    {
        var basket = GetBasket();

        await _basketService.AddToBasketAsync(basket, gameId, quantity);
        
        UpdateCookie(basket);

        return RedirectToAction("GetAll", "Games");
    }

    private Basket GetBasket()
    {
        var basketCookieModel = _basketCookieService.GetBasketFromCookie(RequestCookies);
        var basket = _mapper.Map<Basket>(basketCookieModel);
        
        return basket;
    }

    private void UpdateCookie(Basket basket)
    {
        var basketCookieModel = _mapper.Map<BasketCookieModel>(basket);
        _basketCookieService.AppendBasketCookie(ResponseCookies, basketCookieModel);
    }
}