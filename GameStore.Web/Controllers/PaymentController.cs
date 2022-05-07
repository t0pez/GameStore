using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Orders;
using GameStore.Core.Services.PaymentMethods;
using GameStore.Web.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

[Route("pay")]
public class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly IMapper _mapper;

    public PaymentController(IPaymentService paymentService, IMapper mapper)
    {
        _paymentService = paymentService;
        _mapper = mapper;
    }
    
    [HttpGet("visa")]
    public async Task<ActionResult> VisaPayAsync(OrderViewModel orderViewModel)
    {
        var order = _mapper.Map<Order>(orderViewModel);
        
        var visaPaymentMethod = new VisaPaymentMethod();
        _paymentService.SetPaymentMethod(visaPaymentMethod);

        var paymentResult = await _paymentService.GetPaymentGateway(order);
        var result = paymentResult as VisaPaymentGetaway;

        return View("VisaPaymentStub", result);
    }
    
    [HttpPost("visa-result")]
    public async Task<ActionResult> VisaResultAsync(VisaPaymentGetaway paymentGetaway)
    {
        return RedirectToAction("GetAll", "Orders");
    }
}