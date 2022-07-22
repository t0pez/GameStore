using System;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.ServiceModels.Games;

namespace GameStore.Core.Services;

public class OrderTimeOutService : IOrderTimeOutService
{
    private const int OrderTimeOutOffsetInDays = 3;

    private readonly ISearchService _searchService;
    private readonly IGameService _gameService;
    private readonly IOrderService _orderService;
    private readonly IOpenedOrderService _openedOrderService;
    private readonly IMapper _mapper;

    public OrderTimeOutService(ISearchService searchService, IGameService gameService, IOrderService orderService,
                               IOpenedOrderService openedOrderService, IMapper mapper)
    {
        _searchService = searchService;
        _gameService = gameService;
        _orderService = orderService;
        _openedOrderService = openedOrderService;
        _mapper = mapper;
    }

    public async Task CreateOpenedOrderAsync(Order order)
    {
        var openedOrder = new OpenedOrder
        {
            OrderId = order.Id,
            TimeOutDate = GetTimeOutDate(order.OrderDate)
        };

        if (await _openedOrderService.IsOrderExistsAsync(order.Id))
        {
            await _openedOrderService.UpdateAsync(openedOrder);
            return;
        }
        
        await CreateAsync(order, openedOrder);
    }

    private async Task CreateAsync(Order order, OpenedOrder openedOrder)
    {
        foreach (var orderDetail in order.OrderDetails)
        {
            await ReduceGameQuantity(orderDetail.GameKey, orderDetail.Quantity);
        }

        await _openedOrderService.CreateAsync(openedOrder);
    }

    public async Task RemoveOpenedOrderByOrderIdAsync(Guid orderId)
    {
        var order = await _orderService.GetByIdAsync(orderId);

        foreach (var orderDetail in order.OrderDetails)
        {
            await RestoreGameQuantity(orderDetail.GameKey, orderDetail.Quantity);
        }

        await _openedOrderService.DeleteByOrderIdAsync(orderId);
    }
    
    private async Task ReduceGameQuantity(string gameKey, int quantity)
    {
        await UpdateGameUnitsInStock(gameKey, game => game.UnitsInStock -= quantity);
    }

    private async Task RestoreGameQuantity(string gameKey, int quantity)
    {
        await UpdateGameUnitsInStock(gameKey, game => game.UnitsInStock += quantity);
    }

    private async Task UpdateGameUnitsInStock(string gameKey, Action<ProductDto> updateMethod)
    {
        var game = await _searchService.GetProductDtoByGameKeyOrDefaultAsync(gameKey)
                   ?? throw new ItemNotFoundException(typeof(Game), gameKey);
        updateMethod(game);
        
        var updateModel = _mapper.Map<GameUpdateModel>(game);

        await _gameService.UpdateAsync(updateModel);
    }

    private DateTime GetTimeOutDate(DateTime orderCreateDate)
    {
        return orderCreateDate.AddDays(OrderTimeOutOffsetInDays);
    }
}