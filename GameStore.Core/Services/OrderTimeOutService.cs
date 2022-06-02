using System;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.ServiceModels.Games;

namespace GameStore.Core.Services;

public class OrderTimeOutService : IOrderTimeOutService
{
    private const int OrderTimeOutOffsetInMonths = 3;

    private readonly IGameService _gameService;
    private readonly IOrderService _orderService;
    private readonly IOpenedOrderService _openedOrderService;
    private readonly IMapper _mapper;

    public OrderTimeOutService(IGameService gameService, IOrderService orderService,
                               IOpenedOrderService openedOrderService, IMapper mapper)
    {
        _orderService = orderService;
        _mapper = mapper;
        _gameService = gameService;
        _openedOrderService = openedOrderService;
    }

    public async Task CreateOpenedOrderAsync(Order order)
    {
        var openedOrder = new OpenedOrder
        {
            OrderId = order.Id,
            TimeOutDate = GetTimeOutDate(order.OrderDate)
        };

        foreach (var orderDetail in order.OrderDetails)
        {
            await ReduceGameQuantity(orderDetail.GameId, orderDetail.Quantity);
        }

        await _openedOrderService.CreateAsync(openedOrder);
    }

    public async Task RemoveOpenedOrderByOrderIdAsync(Guid orderId)
    {
        var order = await _orderService.GetByIdAsync(orderId);

        foreach (var orderDetail in order.OrderDetails)
        {
            await RestoreGameQuantity(orderDetail.GameId, orderDetail.Quantity);
        }

        await _openedOrderService.DeleteByOrderIdAsync(orderId);
    }
    
    private async Task ReduceGameQuantity(Guid gameId, int quantity)
    {
        await UpdateGameUnitsInStock(gameId, game => game.UnitsInStock -= quantity);
    }

    private async Task RestoreGameQuantity(Guid gameId, int quantity)
    {
        await UpdateGameUnitsInStock(gameId, game => game.UnitsInStock += quantity);
    }

    private async Task UpdateGameUnitsInStock(Guid gameId, Action<Game> updateMethod)
    {
        var game = await _gameService.GetByIdAsync(gameId);
        updateMethod(game);
        
        var updateModel = _mapper.Map<GameUpdateModel>(game);

        await _gameService.UpdateAsync(updateModel);
    }

    private DateTime GetTimeOutDate(DateTime orderCreateDate)
    {
        return orderCreateDate.AddMonths(OrderTimeOutOffsetInMonths);
    }
}