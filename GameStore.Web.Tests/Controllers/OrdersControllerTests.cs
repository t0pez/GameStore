using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Orders;
using GameStore.Web.Controllers;
using GameStore.Web.Interfaces;
using GameStore.Web.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers;

public class OrdersControllerTests
{
    private readonly OrdersController _ordersController;
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly Mock<IBasketCookieService> _basketCookieServiceMock;
    private readonly Mock<IOrderTimeOutService> _timeOutServiceMock;
    private readonly Mock<IActiveOrderCookieService> _activeOrderCookieServiceMock;
    private readonly Mock<IMapper> _mapperMock;

    public OrdersControllerTests()
    {
        _orderServiceMock = new Mock<IOrderService>();
        _timeOutServiceMock = new Mock<IOrderTimeOutService>();
        _basketCookieServiceMock = new Mock<IBasketCookieService>();
        _timeOutServiceMock = new Mock<IOrderTimeOutService>();
        _activeOrderCookieServiceMock = new Mock<IActiveOrderCookieService>();
        _mapperMock = new Mock<IMapper>();
        _ordersController =
            new OrdersController(_orderServiceMock.Object, _timeOutServiceMock.Object, _basketCookieServiceMock.Object,
                                 _activeOrderCookieServiceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async void GetAllAsync_NoParameters_ReturnsViewWithExpectedResult()
    {
        const int expectedCount = 6;
        var expectedOrders = new List<Order>(new Order[expectedCount]);
        var expectedOrdersViewModels = new List<OrderListViewModel>(new OrderListViewModel[expectedCount]);

        _orderServiceMock.Setup(service => service.GetAllAsync())
                         .ReturnsAsync(expectedOrders);
        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<OrderListViewModel>>(expectedOrdersViewModels))
                   .Returns(expectedOrdersViewModels);

        var actualResult = await _ordersController.GetAllAsync();

        actualResult.Result.Should().BeOfType<ViewResult>().And.Subject.As<ViewResult>().Model
                    .As<IEnumerable<OrderListViewModel>>().Should().HaveCount(expectedCount);
    }

    [Fact]
    public async void GetWithDetailsAsync_CorrectParameter_ReturnsView()
    {
        var expectedOrderId = Guid.NewGuid();
        var expectedOrder = new Order
        {
            Id = expectedOrderId
        };
        var expectedOrderViewModel = new OrderViewModel
        {
            Id = expectedOrderId
        };

        _orderServiceMock.Setup(service => service.GetByIdAsync(expectedOrderId))
                         .ReturnsAsync(expectedOrder);
        _mapperMock.Setup(mapper => mapper.Map<OrderViewModel>(expectedOrder))
                   .Returns(expectedOrderViewModel);

        var actualResult = await _ordersController.GetWithDetailsAsync(expectedOrderId);

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<OrderViewModel>()
                    .And.Subject.As<OrderViewModel>().Id.Should().Be(expectedOrderId);
    }

    [Fact]
    public async void Delete_CorrectValues_ReturnsRedirect()
    {
        var orderId = Guid.NewGuid();

        _orderServiceMock
            .Setup(service => service.DeleteAsync(orderId))
            .Verifiable();
        var actualResult = await _ordersController.DeleteAsync(orderId);

        actualResult.Result.Should().BeOfType<RedirectToActionResult>();
        _orderServiceMock.Verify(service => service.DeleteAsync(orderId), Times.Once);
    }
}