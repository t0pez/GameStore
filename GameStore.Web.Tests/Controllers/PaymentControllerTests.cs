using System;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.PaymentMethods;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.Payment;
using GameStore.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers;

public class PaymentControllerTests
{
    private readonly PaymentController _paymentController;
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly Mock<IPaymentService> _paymentServiceMock;

    public PaymentControllerTests()
    {
        _orderServiceMock = new Mock<IOrderService>();
        _paymentServiceMock = new Mock<IPaymentService>();

        _paymentController = new PaymentController(_paymentServiceMock.Object, _orderServiceMock.Object);
    }

    [Fact]
    public async void VisaPayAsync_ReturnsGetawayView()
    {
        const decimal expectedTotalSum = 100;
        const PaymentType paymentType = PaymentType.Visa;
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId
        };

        _orderServiceMock.Setup(service => service.GetByIdAsync(orderId))
                         .ReturnsAsync(order);
        _paymentServiceMock.Setup(service => service.GetPaymentGateway(order, paymentType))
                           .ReturnsAsync(new VisaPaymentGetaway { TotalSum = expectedTotalSum });

        var actualResult = await _paymentController.VisaPayAsync(orderId);

        actualResult.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<VisaPaymentGetaway>()
                    .And.Subject.As<VisaPaymentGetaway>().TotalSum.Should().Be(expectedTotalSum);
    }
    
    [Fact]
    public async void BankPayAsync_ReturnsFileResult()
    {
        const PaymentType paymentType = PaymentType.Bank;
        
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId
        };

        _orderServiceMock.Setup(service => service.GetByIdAsync(orderId))
                         .ReturnsAsync(order);
        _paymentServiceMock.Setup(service => service.GetPaymentGateway(order, paymentType))
                           .ReturnsAsync(new BankPaymentGetaway { InvoiceFileContent = new byte[] { 0, 1, 0, 1 } });

        var actualResult = await _paymentController.BankPayAsync(orderId);

        actualResult.Should().BeOfType<FileContentResult>();
    }
    
    [Fact]
    public async void IboxPayAsync_ReturnsGetawayView()
    {
        const decimal expectedTotalSum = 100;
        const PaymentType paymentType = PaymentType.Ibox;
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId
        };

        _orderServiceMock.Setup(service => service.GetByIdAsync(orderId))
                         .ReturnsAsync(order);
        _paymentServiceMock.Setup(service => service.GetPaymentGateway(order, paymentType))
                           .ReturnsAsync(new IboxPaymentGetaway { OrderId = orderId, TotalSum = expectedTotalSum });

        var actualResult = await _paymentController.IboxPayAsync(orderId);

        actualResult.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<IboxPaymentGetaway>()
                    .And.Subject.As<IboxPaymentGetaway>().OrderId.Should().Be(orderId);
    }

    [Fact]
    public async void ResultStubAsync_ReturnsRedirect()
    {
        var actualResult = await _paymentController.ResultStubAsync(new VisaPaymentGetaway());

        actualResult.Should().BeOfType<RedirectToActionResult>();
    }
}