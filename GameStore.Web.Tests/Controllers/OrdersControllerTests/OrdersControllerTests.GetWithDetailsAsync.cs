using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Server.Orders;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using GameStore.Web.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.OrdersControllerTests;

public partial class OrdersControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void GetWithDetailsAsync_CorrectParameter_ReturnsView(
        Order order,
        [Frozen] Mock<IOrderService> orderServiceMock,
        OrdersController sut)
    {
        orderServiceMock.Setup(service => service.GetByIdAsync(order.Id))
                        .ReturnsAsync(order);

        var actualResult = await sut.GetWithDetailsAsync(order.Id);

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<OrderViewModel>()
                    .And.Subject.As<OrderViewModel>().Id.Should().Be(order.Id);
    }
}