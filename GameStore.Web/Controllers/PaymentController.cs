using System;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Extensions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.PaymentMethods;
using GameStore.Core.Models.Orders;
using GameStore.Core.Services.PaymentMethods;
using GameStore.Web.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

[Route("pay")]
public class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly IOrderService _orderService;
    private readonly IMapper _mapper;

    public PaymentController(IPaymentService paymentService, IMapper mapper, IOrderService orderService)
    {
        _paymentService = paymentService;
        _orderService = orderService;
        _mapper = mapper;
    }
    
    [HttpPost("visa")]
    public async Task<ActionResult> VisaPayAsync(OrderViewModel orderViewModel)
    {
        var order = _mapper.Map<Order>(orderViewModel);
        
        var paymentResult = await _paymentService.GetPaymentGateway(order, PaymentType.Visa);
        var result = paymentResult as VisaPaymentGetaway;

        return View("VisaPaymentStub", result);
    }
    
    [HttpPost("bank")]
    public async Task<FileContentResult> BankPayAsync(Guid orderId)
    {
        var order = await _orderService.GetByIdAsync(orderId);
        
        var paymentResult = await _paymentService.GetPaymentGateway(order, PaymentType.Bank);
        var result = paymentResult as BankPaymentGetaway;

        return File(result.InvoiceFileContent.ToByteArray(), "application/force-download", "invoice-file.txt");
    }
    
    [HttpPost("ibox")]
    public async Task<ActionResult> IboxPayAsync(OrderViewModel orderViewModel)
    {
        var order = _mapper.Map<Order>(orderViewModel);
        
        var paymentResult = await _paymentService.GetPaymentGateway(order, PaymentType.Ibox);
        var result = paymentResult as IboxPaymentGetaway;

        return View("IboxPaymentStub", result);
    }
    
    [HttpPost("payment-result")]
    public async Task<ActionResult> VisaResultAsync(VisaPaymentGetaway paymentGetaway)
    {
        return RedirectToAction("GetAll", "Orders");
    }
}