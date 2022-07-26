using System;
using System.Threading.Tasks;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.PaymentMethods;
using GameStore.Core.Models.Payment;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

[Route("pay")]
public class PaymentController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService, IOrderService orderService)
    {
        _paymentService = paymentService;
        _orderService = orderService;
    }

    [HttpGet("visa")]
    public async Task<ActionResult> VisaPayAsync(Guid orderId)
    {
        var paymentGetaway = await GetPaymentGetaway(orderId, PaymentType.Visa);
        var result = paymentGetaway as VisaPaymentGetaway;

        return View("VisaPaymentStub", result);
    }

    [HttpGet("bank")]
    public async Task<FileContentResult> BankPayAsync(Guid orderId)
    {
        var paymentGetaway = await GetPaymentGetaway(orderId, PaymentType.Bank);
        var result = paymentGetaway as BankPaymentGetaway;

        return File(result.InvoiceFileContent, "application/pdf", "invoice-file.pdf");
    }

    [HttpGet("ibox")]
    public async Task<ActionResult> IboxPayAsync(Guid orderId)
    {
        var paymentGetaway = await GetPaymentGetaway(orderId, PaymentType.Ibox);
        var result = paymentGetaway as IboxPaymentGetaway;

        return View("IboxPaymentStub", result);
    }

    [HttpPost("payment-result")]
    public async Task<ActionResult> ResultStubAsync(VisaPaymentGetaway paymentGetaway)
    {
        return RedirectToAction("GetByFilter", "Orders");
    }

    private async Task<PaymentGetaway> GetPaymentGetaway(Guid orderId, PaymentType paymentType)
    {
        var order = await _orderService.GetByIdAsync(orderId);
        var paymentGateway = await _paymentService.GetPaymentGateway(order, paymentType);

        return paymentGateway;
    }
}