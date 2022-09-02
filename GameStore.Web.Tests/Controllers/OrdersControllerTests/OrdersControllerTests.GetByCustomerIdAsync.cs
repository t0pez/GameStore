using System;
using System.Collections.Generic;
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
    public async void GetByCustomerIdAsync_CorrectParameter_ReturnsView(
        Guid customerId,
        List<Order> orders,
        [Frozen] Mock<IOrderService> orderServiceMock,
        OrdersController sut)
    {
        orderServiceMock.Setup(service => service.GetByCustomerIdAsync(customerId))
                        .ReturnsAsync(orders);

        var actualResult = await sut.GetByCustomerIdAsync(customerId);

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<List<OrderListViewModel>>()
                    .And.Subject.As<List<OrderListViewModel>>().Count.Should().Be(orders.Count);
    }
}