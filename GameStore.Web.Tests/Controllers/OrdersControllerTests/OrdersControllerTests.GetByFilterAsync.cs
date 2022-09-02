using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using GameStore.Web.Models.Order;
using GameStore.Web.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.OrdersControllerTests;

public partial class OrdersControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void GetByFilterAsync_NoParameters_ReturnsViewWithExpectedResult(
        AllOrdersFilterRequestModel filterRequest,
        List<OrderDto> orders,
        [Frozen] Mock<IOrderService> orderServiceMock,
        OrdersController sut)
    {
        orderServiceMock.Setup(service => service.GetByFilterAsync(It.IsAny<AllOrdersFilter>()))
                        .ReturnsAsync(orders);

        var actualResult = await sut.GetByFilterAsync(filterRequest);

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .And.Subject.As<ViewResult>().Model
                    .As<GetAllViewModel>().Orders.Should().HaveCount(orders.Count);
    }
}