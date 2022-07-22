using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.Loggers;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Core.Models.Mongo.Orders;
using GameStore.Core.Models.Mongo.Orders.Filters;
using GameStore.Core.Models.Mongo.Orders.Specifications;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.Orders.Filters;
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
    private readonly Mock<ISearchService> _searchServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepository<Order>> _orderRepoMock;
    private readonly Mock<IRepository<OrderMongo>> _orderMongoRepoMock;

    public OrderServiceTests()
    {
        var mongoLogger = new Mock<IMongoLogger>();
        _searchServiceMock = new Mock<ISearchService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _orderRepoMock = new Mock<IRepository<Order>>();
        _orderMongoRepoMock = new Mock<IRepository<OrderMongo>>();

        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.GetEfRepository<Order>())
                       .Returns(_orderRepoMock.Object);
        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.GetMongoRepository<OrderMongo>())
                       .Returns(_orderMongoRepoMock.Object);
        
        _orderService = new OrderService(_searchServiceMock.Object, mongoLogger.Object, _unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async void GetByFilterAsync_ReturnsCorrectResult()
    {
        const int expectedCount = 4;
        var filter = new AllOrdersFilter();
        var serverFilter = new OrdersFilter();
        var mongoFilter = new MongoOrdersFilter();
        
        var expectedServerItems = new List<Order>
        {
            new()
            {
                CustomerId = "1"
            },
            new()
            {
                CustomerId = "2"
            }
        };
        var expectedMongoItems = new List<OrderMongo>
        {
            new()
            {
                CustomerId = "3"
            },
            new()
            {
                CustomerId = "4"
            }
        };
        var mappedServerItems = new List<OrderDto>
        {
            new()
            {
                CustomerId = "1"
            },
            new()
            {
                CustomerId = "2"
            }
        };
        var mappedMongoItems = new List<OrderDto>
        {
            new()
            {
                CustomerId = "3"
            },
            new()
            {
                CustomerId = "4"
            }
        };

        _orderRepoMock.Setup(repository => repository.GetBySpecAsync(It.IsAny<OrdersByFilterSpec>()))
                      .ReturnsAsync(expectedServerItems);
        _orderMongoRepoMock.Setup(repository => repository.GetBySpecAsync(It.IsAny<MongoOrdersByFilterWithDetailsSpec>()))
                      .ReturnsAsync(expectedMongoItems);
        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<OrderDto>>(expectedServerItems))
                   .Returns(mappedServerItems);
        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<OrderDto>>(expectedMongoItems))
                   .Returns(mappedMongoItems);
        _mapperMock.Setup(mapper => mapper.Map<OrdersFilter>(filter))
                   .Returns(serverFilter);
        _mapperMock.Setup(mapper => mapper.Map<MongoOrdersFilter>(filter))
                   .Returns(mongoFilter);
        

        var actualResult = await _orderService.GetByFilterAsync(filter);

        actualResult.Count.Should().Be(expectedCount);
    }
    
    [Fact]
    public async void GetByIdAsync_ExistingOrderId_ReturnsCorrectResult()
    {
        var orderId = Guid.NewGuid();
        var expectedResult = new Order { Id = orderId };

        _orderRepoMock.Setup(repository =>
                                 repository.GetSingleOrDefaultBySpecAsync(
                                     It.Is<OrderByIdWithDetailsSpec>(spec => spec.Id == orderId)))
                      .ReturnsAsync(expectedResult);

        var actualResult = await _orderService.GetByIdAsync(orderId);

        actualResult.Id.Should().Be(orderId);
    }
    
    [Fact]
    public async void GetByCustomerIdAsync_ExistingCustomer_ReturnsCorrectResult()
    {
        const int expectedCount = 4;
        var customerId = Guid.NewGuid().ToString();
        var expectedResult = new List<Order>(new Order[expectedCount]);

        _orderRepoMock.Setup(repository => repository.GetBySpecAsync(It.Is<OrdersByCustomerIdSpec>(spec => spec.CustomerId == customerId)))
                      .ReturnsAsync(expectedResult);

        var actualResult = await _orderService.GetByCustomerIdAsync(customerId);

        actualResult.Count.Should().Be(expectedCount);
    }

    [Fact]
    public async void CreateAsync_CorrectValues_ReturnsOrder()
    {
        const string customerId = "customerId";
        const bool customerHasActiveOrder = false;
        
        var orderCreateModel = new OrderCreateModel { CustomerId = customerId };
        var order = new Order { CustomerId = customerId };

        _orderRepoMock
            .Setup(repository =>
                       repository.AnyAsync(
                           It.Is<OrderInProcessByCustomerIdSpec>(spec => spec.CustomerId == customerId)))
            .ReturnsAsync(customerHasActiveOrder);
        _mapperMock.Setup(mapper => mapper.Map<Order>(orderCreateModel))
                   .Returns(order);

        var actualResult = await _orderService.CreateAsync(orderCreateModel);

        actualResult.CustomerId.Should().Be(customerId);
        _orderRepoMock.Verify(repository => repository.AddAsync(order), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async void UpdateAsync_CorrectValues_UpdatesOrder()
    {
        var orderId = Guid.NewGuid();
        var orderUpdateModel = new OrderUpdateModel { Id = orderId };

        var order = new Order { Id = orderId };

        _orderRepoMock.Setup(repository =>
                                 repository.GetSingleOrDefaultBySpecAsync(
                                     It.Is<OrderByIdSpec>(spec => spec.Id == orderId)))
                      .ReturnsAsync(order);

        await _orderService.UpdateAsync(orderUpdateModel);
        
        _orderRepoMock.Verify(repository => repository.UpdateAsync(order), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
    }
}