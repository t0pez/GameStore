using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Baskets;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.ServiceModels.Orders;
using GameStore.Web.Interfaces;
using GameStore.Web.Models.Baskets;
using GameStore.Web.Models.Order;
using GameStore.Web.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

[Route("orders")]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IOrderTimeOutService _timeOutService;
    private readonly IBasketCookieService _basketCookieService;
    private readonly IMapper _mapper;

    public OrdersController(IOrderService orderService, IOrderTimeOutService timeOutService,
                            IBasketCookieService basketCookieService, IMapper mapper)
    {
        _orderService = orderService;
        _mapper = mapper;
        _basketCookieService = basketCookieService;
        _timeOutService = timeOutService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderListViewModel>>> GetAllAsync()
    {
        var orders = await _orderService.GetAllAsync();

        var result = _mapper.Map<IEnumerable<OrderListViewModel>>(orders);

        return View(result);
    }
    
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<OrderListViewModel>>> GetByCustomerIdAsync(Guid customerId)
    {
        var orders = await _orderService.GetByCustomerIdAsync(customerId);

        var result = _mapper.Map<IEnumerable<OrderListViewModel>>(orders);

        return View(result);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderViewModel>> GetWithDetailsAsync(Guid id)
    {
        var order = await _orderService.GetByIdAsync(id);

        var result = _mapper.Map<OrderViewModel>(order);

        return View(result);
    }

    [HttpGet("new")]
    public async Task<ActionResult<OrderViewModel>> CreateAsync()
    {
        var basketCookieModel = _basketCookieService.GetBasketFromCookie(HttpContext.Request.Cookies);
        var basket = _mapper.Map<Basket>(basketCookieModel);
        
        var createModel = new OrderCreateModel { Basket = basket };
        
        var order = await _orderService.CreateAsync(createModel);
        await _timeOutService.CreateOpenedOrderAsync(order);

        basketCookieModel = new BasketCookieModel(); 
        _basketCookieService.AppendBasketCookie(HttpContext.Response.Cookies, basketCookieModel);
        
        var result = _mapper.Map<OrderViewModel>(order);

        return View("Checkout", result);
    }

    [HttpGet("{id}/update")]
    public async Task<ActionResult<OrderUpdateRequestModel>> UpdateAsync(Guid id)
    {
        var order = await _orderService.GetByIdAsync(id);

        var result = _mapper.Map<OrderUpdateRequestModel>(order);

        return View(result);
    }
    
    [HttpPost("{id}/update")]
    public async Task<ActionResult<OrderViewModel>> UpdateAsync(OrderUpdateRequestModel requestModel)
    {
        var updateModel = _mapper.Map<OrderUpdateModel>(requestModel);
        
        if (updateModel.Status is OrderStatus.Completed or OrderStatus.Cancelled)
        {
            await _timeOutService.RemoveOpenedOrderByOrderIdAsync(updateModel.Id);
        }
        
        await _orderService.UpdateAsync(updateModel);

        return RedirectToAction("GetWithDetails", "Orders", new { id = requestModel.Id });
    }

    [HttpPost("{id}/delete")]
    public async Task<ActionResult<IEnumerable<OrderListViewModel>>> DeleteAsync(Guid id)
    {
        await _orderService.DeleteAsync(id);
        await _timeOutService.RemoveOpenedOrderByOrderIdAsync(id);

        return RedirectToAction("GetAll", "Orders");
    }
}