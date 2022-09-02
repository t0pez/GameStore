using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.PaymentMethods;
using GameStore.Core.Models.Payment;
using GameStore.Core.Models.Server.Orders;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.PaymentControllerTests;

public partial class PaymentControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void IboxPayAsync_ReturnsGetawayView(
        Order order,
        [Frozen] Mock<IOrderService> orderServiceMock,
        [Frozen] Mock<IPaymentService> paymentServiceMock,
        PaymentController sut)
    {
        const PaymentType paymentType = PaymentType.Ibox;

        orderServiceMock.Setup(service => service.GetByIdAsync(order.Id))
                        .ReturnsAsync(order);

        paymentServiceMock.Setup(service => service.GetPaymentGateway(order, paymentType))
                          .ReturnsAsync(new IboxPaymentGetaway { OrderId = order.Id, TotalSum = order.TotalSum });

        var actualResult = await sut.IboxPayAsync(order.Id);

        actualResult.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<IboxPaymentGetaway>()
                    .And.Subject.As<IboxPaymentGetaway>().OrderId.Should().Be(order.Id);
    }
}