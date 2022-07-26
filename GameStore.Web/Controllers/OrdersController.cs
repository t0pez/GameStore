using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.PaymentMethods;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Core.Models.Mongo.Shippers;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.ServiceModels.Orders;
using GameStore.Web.Interfaces;
using GameStore.Web.Models.Order;
using GameStore.Web.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.Web.Controllers;

[Route("orders")]
public class OrdersController : Controller
{
    private readonly IMapper _mapper;
    private readonly IOrderService _orderService;
    private readonly IShipperService _shipperService;
    private readonly IOrderTimeOutService _timeOutService;
    private readonly IUserCookieService _userCookieService;

    public OrdersController(IOrderService orderService, IShipperService shipperService,
                            IOrderTimeOutService timeOutService,
                            IUserCookieService userCookieService,
                            IMapper mapper)
    {
        _orderService = orderService;
        _shipperService = shipperService;
        _timeOutService = timeOutService;
        _userCookieService = userCookieService;
        _mapper = mapper;
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<OrderListViewModel>>> GetByFilterAsync(
        AllOrdersFilterRequestModel filterRequest)
    {
        var filter = _mapper.Map<AllOrdersFilter>(filterRequest);
        var orders = await _orderService.GetByFilterAsync(filter);

        var result = _mapper.Map<IEnumerable<OrderListViewModel>>(orders);

        return View(result);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<OrderListViewModel>>> GetByCustomerIdAsync(string customerId)
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

    [HttpGet("basket")]
    public async Task<ActionResult<OrderViewModel>> GetCurrentBasketAsync()
    {
        if (_userCookieService.TryGetCookiesUserId(HttpContext.Request.Cookies, out var customerId) == false)
        {
            customerId = Guid.NewGuid().ToString();
            _userCookieService.AppendUserId(HttpContext.Response.Cookies, customerId);
        }

        var order = await _orderService.GetBasketByCustomerIdAsync(customerId);
        var result = _mapper.Map<OrderViewModel>(order);

        if (order.Status == OrderStatus.InProcess)
        {
            return RedirectToAction("Payment", "Orders", new { orderId = order.Id });
        }

        ViewData["HasActiveOrder"] = await _orderService.IsCustomerHasActiveOrder(customerId);

        return View(result);
    }

    [HttpPost("/games/{gameKey}/buy")]
    public async Task<ActionResult> AddToBasketAsync(string gameKey, int quantity)
    {
        if (_userCookieService.TryGetCookiesUserId(HttpContext.Request.Cookies, out var customerId) == false)
        {
            customerId = Guid.NewGuid().ToString();
            _userCookieService.AppendUserId(HttpContext.Response.Cookies, customerId);
        }

        var basketItem = new BasketItem
        {
            GameKey = gameKey,
            Quantity = quantity
        };

        await _orderService.AddToOrderAsync(customerId, basketItem);

        return RedirectToAction("GetWithDetails", "Games", new { gameKey });
    }

    [HttpGet("{orderId}/make-order")]
    public async Task<ActionResult> MakeOrderAsync(Guid orderId)
    {
        await _orderService.MakeOrder(orderId);

        return RedirectToAction("UpdateShipInfo", "Orders", new { orderId });
    }

    [HttpGet("{orderId}/ship-info")]
    public async Task<ActionResult<ActiveOrderCreateRequestModel>> UpdateShipInfoAsync(Guid orderId)
    {
        var shippers = await _shipperService.GetAll();

        var shippersSelectList = new SelectList(shippers, nameof(Shipper.ShipperId), nameof(Shipper.CompanyName));
        ViewData["Shippers"] = shippersSelectList;

        return View(new ActiveOrderCreateRequestModel { OrderId = orderId });
    }

    [HttpPost("{orderId}/ship-info")]
    public async Task<ActionResult> UpdateShipInfoAsync(ActiveOrderCreateRequestModel requestModel)
    {
        var createModel = _mapper.Map<ActiveOrderCreateModel>(requestModel);

        await _orderService.FillShippersAsync(createModel);

        return RedirectToAction("Payment", "Orders", new { requestModel.OrderId });
    }

    [HttpGet("{orderId}/payment")]
    public async Task<ActionResult> PaymentAsync(Guid orderId)
    {
        var order = await _orderService.GetByIdAsync(orderId);
        var orderViewModel = _mapper.Map<OrderViewModel>(order);

        return View("Checkout", orderViewModel);
    }

    [HttpPost("{orderId}/payment")]
    public async Task<ActionResult> PaymentAsync(Guid orderId, PaymentType paymentType)
    {
        var order = await _orderService.GetByIdAsync(orderId);

        await _timeOutService.CreateOpenedOrderAsync(order);

        return RedirectToPayment(orderId, paymentType);
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

    private ActionResult RedirectToPayment(Guid orderId, PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.Visa => RedirectToAction("VisaPay", "Payment", new { orderId }),
            PaymentType.Ibox => RedirectToAction("IboxPay", "Payment", new { orderId }),
            PaymentType.Bank => RedirectToAction("BankPay", "Payment", new { orderId }),
            _                => throw new ArgumentOutOfRangeException(nameof(paymentType), paymentType, null)
        };
    }
}