using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.ServiceModels.Orders;
using GameStore.Web.Controllers;
using GameStore.Web.Interfaces;
using GameStore.Web.Models.Order;
using GameStore.Web.ViewModels.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers;

public class OrdersControllerTests
{
    private readonly OrdersController _ordersController;
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly Mock<IShipperService> _shipperServiceMock;
    private readonly Mock<IOrderTimeOutService> _timeOutServiceMock;
    private readonly Mock<IUserCookieService> _userCookieServiceMock;
    private readonly Mock<IMapper> _mapperMock;

    public OrdersControllerTests()
    {
        _orderServiceMock = new Mock<IOrderService>();
        _timeOutServiceMock = new Mock<IOrderTimeOutService>();
        _shipperServiceMock = new Mock<IShipperService>();
        _userCookieServiceMock = new Mock<IUserCookieService>();
        _mapperMock = new Mock<IMapper>();
        _ordersController = new OrdersController(_orderServiceMock.Object, _shipperServiceMock.Object,
                                                 _timeOutServiceMock.Object, _userCookieServiceMock.Object,
                                                 _mapperMock.Object);
        
        _ordersController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async void GetByFilterAsync_NoParameters_ReturnsViewWithExpectedResult()
    {
        const int expectedCount = 6;
        var expectedOrders = new List<OrderDto>(new OrderDto[expectedCount]);
        var expectedOrdersViewModels = new List<OrderListViewModel>(new OrderListViewModel[expectedCount]);

        var filter = new AllOrdersFilterRequestModel();
        var ordersFilter = new AllOrdersFilter();
        
        _orderServiceMock.Setup(service => service.GetByFilterAsync(ordersFilter))
                         .ReturnsAsync(expectedOrders);
        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<OrderListViewModel>>(expectedOrders))
                   .Returns(expectedOrdersViewModels);
        _mapperMock.Setup(mapper => mapper.Map<AllOrdersFilter>(filter))
                   .Returns(ordersFilter);

        var actualResult = await _ordersController.GetByFilterAsync(filter);

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .And.Subject.As<ViewResult>().Model
                    .As<IEnumerable<OrderListViewModel>>().Should().HaveCount(expectedCount);
    }

    [Fact]
    public async void GetByCustomerIdAsync_CorrectParameter_ReturnsView()
    {
        var customerId = Guid.NewGuid().ToString();
        const int expectedCount = 4;
        var expectedOrders = new List<Order>(new Order[expectedCount]);
        var expectedOrderViewModels = new List<OrderListViewModel>(new OrderListViewModel[expectedCount]);

        _orderServiceMock.Setup(service => service.GetByCustomerIdAsync(customerId))
                         .ReturnsAsync(expectedOrders);
        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<OrderListViewModel>>(expectedOrders))
                   .Returns(expectedOrderViewModels);

        var actualResult = await _ordersController.GetByCustomerIdAsync(customerId);

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<List<OrderListViewModel>>()
                    .And.Subject.As<List<OrderListViewModel>>().Count.Should().Be(expectedCount);
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
    public async void UpdateAsync_CorrectId_ReturnsView()
    {
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId };
        var orderUpdateRequestModel = new OrderUpdateRequestModel { Id = orderId };

        _orderServiceMock.Setup(service => service.GetByIdAsync(orderId))
                         .ReturnsAsync(order);
        _mapperMock.Setup(mapper => mapper.Map<OrderUpdateRequestModel>(order))
                   .Returns(orderUpdateRequestModel);

        var actualResult = await _ordersController.UpdateAsync(orderId);
        
        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<OrderUpdateRequestModel>()
                    .And.Subject.As<OrderUpdateRequestModel>().Id.Should().Be(orderId);
    }
    
    [Fact]
    public async void UpdateAsync_CorrectValues_ReturnsRedirect()
    {
        var orderId = Guid.NewGuid();
        var orderUpdateRequestModel = new OrderUpdateRequestModel { Id = orderId };
        var orderUpdateModel = new OrderUpdateModel { Id = orderId };
        
        _mapperMock.Setup(mapper => mapper.Map<OrderUpdateModel>(orderUpdateRequestModel))
                   .Returns(orderUpdateModel);

        var actualResult = await _ordersController.UpdateAsync(orderUpdateRequestModel);

        actualResult.Result.Should().BeOfType<RedirectToActionResult>();
    }
}