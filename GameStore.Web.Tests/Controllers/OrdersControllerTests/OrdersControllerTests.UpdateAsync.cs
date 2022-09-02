using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Server.Orders;
using GameStore.Core.Models.ServiceModels.Orders;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using GameStore.Web.Models.Order;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.OrdersControllerTests;

public partial class OrdersControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void UpdateAsync_CorrectId_ReturnsView(
        Order order,
        [Frozen] Mock<IOrderService> orderServiceMock,
        OrdersController sut)
    {
        orderServiceMock.Setup(service => service.GetByIdAsync(order.Id))
                        .ReturnsAsync(order);

        var actualResult = await sut.UpdateAsync(order.Id);

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<OrderUpdateRequestModel>()
                    .And.Subject.As<OrderUpdateRequestModel>().Id.Should().Be(order.Id);
    }

    [Theory]
    [AutoMoqData]
    public async void UpdateAsync_CorrectValues_ReturnsRedirect(
        OrderUpdateRequestModel requestModel,
        [Frozen] Mock<IOrderService> orderServiceMock,
        OrdersController sut)
    {
        var actualResult = await sut.UpdateAsync(requestModel);

        actualResult.Result.Should().BeOfType<RedirectToActionResult>();

        orderServiceMock.Verify(service => service.UpdateAsync(It.IsAny<OrderUpdateModel>()));
    }
}