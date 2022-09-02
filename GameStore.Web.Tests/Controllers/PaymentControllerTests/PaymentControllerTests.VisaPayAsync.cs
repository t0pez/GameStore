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
    public async void VisaPayAsync_ReturnsGetawayView(
        Order order,
        [Frozen] Mock<IOrderService> orderServiceMock,
        [Frozen] Mock<IPaymentService> paymentServiceMock,
        PaymentController sut)
    {
        const PaymentType paymentType = PaymentType.Visa;

        orderServiceMock.Setup(service => service.GetByIdAsync(order.Id))
                        .ReturnsAsync(order);

        paymentServiceMock.Setup(service => service.GetPaymentGateway(order, paymentType))
                          .ReturnsAsync(new VisaPaymentGetaway { TotalSum = order.TotalSum });

        var actualResult = await sut.VisaPayAsync(order.Id);

        actualResult.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<VisaPaymentGetaway>()
                    .And.Subject.As<VisaPaymentGetaway>().TotalSum.Should().Be(order.TotalSum);
    }
}