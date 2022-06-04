using System;
using System.Collections.Generic;
using FluentAssertions;
using GameStore.Core.Helpers.OrderMapping;
using GameStore.Core.Models.Baskets;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.Orders.Specifications;
using GameStore.Core.Models.ServiceModels.Orders;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services;

public class OrderServiceTests
{
    private readonly OrderService _orderService;
    private readonly Mock<IOrderMappingHelper> _orderMappingHelperMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Order>> _orderRepoMock;

    public OrderServiceTests()
    {
        _orderMappingHelperMock = new Mock<IOrderMappingHelper>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _orderRepoMock = new Mock<IRepository<Order>>();

        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.GetRepository<Order>())
                       .Returns(_orderRepoMock.Object);
        
        _orderService = new OrderService(_orderMappingHelperMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async void GetAllAsync_NoParameters_ReturnsCorrectResult()
    {
        const int expectedCount = 4;
        var expectedResult = new List<Order>(new Order[expectedCount]);

        _orderRepoMock.Setup(repository => repository.GetBySpecAsync(It.IsAny<OrdersListSpec>()))
                      .ReturnsAsync(expectedResult);

        var actualResult = await _orderService.GetAllAsync();

        actualResult.Count.Should().Be(expectedCount);
    }
    
    [Fact]
    public async void GetByIdAsync_ExistingOrderId_ReturnsCorrectResult()
    {
        var orderId = Guid.NewGuid();
        var expectedResult = new Order { Id = orderId };

        _orderRepoMock.Setup(repository => repository.GetSingleOrDefaultBySpecAsync(It.Is<OrderByIdWithDetailsSpec>(spec => spec.Id == orderId)))
                      .ReturnsAsync(expectedResult);

        var actualResult = await _orderService.GetByIdAsync(orderId);

        actualResult.Id.Should().Be(orderId);
    }
    
    [Fact]
    public async void GetByCustomerIdAsync_ExistingCustomer_ReturnsCorrectResult()
    {
        const int expectedCount = 4;
        var customerId = Guid.NewGuid();
        var expectedResult = new List<Order>(new Order[expectedCount]);

        _orderRepoMock.Setup(repository => repository.GetBySpecAsync(It.Is<OrdersByCustomerIdSpec>(spec => spec.CustomerId == customerId)))
                      .ReturnsAsync(expectedResult);

        var actualResult = await _orderService.GetByCustomerIdAsync(customerId);

        actualResult.Count.Should().Be(expectedCount);
    }

    [Fact]
    public async void CreateAsync_CorrectValues_ReturnsOrder()
    {
        var orderCreateModel = new OrderCreateModel();
        
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId };

        _orderMappingHelperMock.Setup(helper => helper.GetOrderAsync(It.IsAny<Basket>()))
                               .ReturnsAsync(order);

        var actualResult = await _orderService.CreateAsync(orderCreateModel);

        actualResult.Id.Should().Be(orderId);
        _orderRepoMock.Verify(repository => repository.AddAsync(order), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async void UpdateAsync_CorrectValues_UpdatesOrder()
    {
        var orderId = Guid.NewGuid();
        var orderUpdateModel = new OrderUpdateModel { Id = orderId };

        var order = new Order { Id = orderId };

        _orderRepoMock.Setup(repository => repository.GetSingleOrDefaultBySpecAsync(It.Is<OrderByIdSpec>(spec => spec.Id == orderId)))
                      .ReturnsAsync(order);

        await _orderService.UpdateAsync(orderUpdateModel);
        
        _orderRepoMock.Verify(repository => repository.UpdateAsync(order), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async void DeleteAsync_CorrectValues_OrderMarkedAsDeleted()
    {
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId };

        _orderRepoMock.Setup(repository => repository.GetSingleOrDefaultBySpecAsync(It.Is<OrderByIdSpec>(spec => spec.Id == orderId)))
                      .ReturnsAsync(order);

        await _orderService.DeleteAsync(orderId);
        
        _orderRepoMock.Verify(repository => repository.UpdateAsync(order), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
    }
}