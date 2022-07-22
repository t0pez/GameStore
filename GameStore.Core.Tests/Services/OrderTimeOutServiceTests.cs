using System;
using System.Collections.Generic;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.Core.Services;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services;

public class OrderTimeOutServiceTests
{
    private readonly OrderTimeOutService _timeOutService;
    private readonly Mock<IGameService> _gameServiceMock;
    private readonly Mock<ISearchService> _searchServiceMock;
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly Mock<IOpenedOrderService> _openedOrderServiceMock;
    private readonly Mock<IMapper> _mapperMock;

    public OrderTimeOutServiceTests()
    {
        _searchServiceMock = new Mock<ISearchService>();
        _gameServiceMock = new Mock<IGameService>();
        _orderServiceMock = new Mock<IOrderService>();
        _openedOrderServiceMock = new Mock<IOpenedOrderService>();
        _mapperMock = new Mock<IMapper>();

        _timeOutService = new OrderTimeOutService(_searchServiceMock.Object, _gameServiceMock.Object,
                                                  _orderServiceMock.Object, _openedOrderServiceMock.Object,
                                                  _mapperMock.Object);
    }

    [Fact]
    public async void CreateOpenedOrderAsync_OrderAsAParameter_ReducesGameUnitsInStock()
    {
        var orderId = Guid.NewGuid();
        var orderDate = DateTime.Now.AddDays(-3);
        const string gameKey = "game-key";

        var game = new ProductDto { Key = gameKey, UnitsInStock = 100 };
        var gameUpdateModel = new GameUpdateModel { Key = gameKey};
        
        var orderDetail = new OrderDetails { GameKey = gameKey, Quantity = 1 };
        var orderDetails = new List<OrderDetails> { orderDetail };
        var order = new Order
        {
            Id = orderId,
            OrderDate = orderDate,
            OrderDetails = orderDetails
        };

        _searchServiceMock.Setup(service => service.GetProductDtoByGameKeyOrDefaultAsync(gameKey))
                        .ReturnsAsync(game);
        _mapperMock.Setup(mapper => mapper.Map<GameUpdateModel>(game))
                   .Returns(gameUpdateModel);

        await _timeOutService.CreateOpenedOrderAsync(order);
        
        _gameServiceMock.Verify(service => service.UpdateAsync(gameUpdateModel), Times.Once);
        _openedOrderServiceMock.Verify(service => service.CreateAsync(It.Is<OpenedOrder>(openedOrder => openedOrder.OrderId == orderId)));
    }

    [Fact]
    public async void RemoveOpenedOrderByOderIdAsync_ExistingOrder_RestoresGameUnitsInStock()
    {
        var orderId = Guid.NewGuid();
        const string gameKey = "game-key";

        var game = new ProductDto { Key = gameKey, UnitsInStock = 100 };
        var gameUpdateModel = new GameUpdateModel { Key = gameKey};
        
        var orderDetail = new OrderDetails { GameKey = gameKey, Quantity = 1 };
        var orderDetails = new List<OrderDetails> { orderDetail };
        var order = new Order
        {
            Id = orderId,
            OrderDetails = orderDetails
        };

        _orderServiceMock.Setup(service => service.GetByIdAsync(orderId))
                         .ReturnsAsync(order);
        _searchServiceMock.Setup(service => service.GetProductDtoByGameKeyOrDefaultAsync(gameKey))
                          .ReturnsAsync(game);
        _mapperMock.Setup(mapper => mapper.Map<GameUpdateModel>(game))
                   .Returns(gameUpdateModel);

        await _timeOutService.RemoveOpenedOrderByOrderIdAsync(orderId);
        
        _gameServiceMock.Verify(service => service.UpdateAsync(gameUpdateModel), Times.Once);
        _openedOrderServiceMock.Verify(service => service.DeleteByOrderIdAsync(orderId), Times.Once);
    }
}