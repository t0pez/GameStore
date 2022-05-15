using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Core.Exceptions;
using GameStore.Core.Models.Baskets;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.Core.Models.Orders;
using GameStore.SharedKernel.Interfaces.DataAccess;

namespace GameStore.Core.Helpers.OrderMapping;

public class OrderMappingHelper : IOrderMappingHelper
{
    private const decimal DefaultDiscount = 0m;
    private readonly IUnitOfWork _unitOfWork;

    public OrderMappingHelper(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Order> GetOrderAsync(Basket basket)
    {
        var order = new Order();

        var orderDetails = await GetOrderDetailsAsync(basket.Items);

        order.CustomerId = basket.CustomerId;
        order.OrderDetails = orderDetails;

        return order;
    }

    private async Task<ICollection<OrderDetails>> GetOrderDetailsAsync(IEnumerable<BasketItem> basketItems)
    {
        var result = basketItems.Select(async item => await GetSingleOrderDetailsAsync(item))
                                .Select(task => task.Result);

        return result.ToList();
    }

    private async Task<OrderDetails> GetSingleOrderDetailsAsync(BasketItem basketItem)
    {
        var orderDetails = new OrderDetails();
        
        var gameRepository = _unitOfWork.GetRepository<Game>();
        var game = await gameRepository.GetSingleOrDefaultBySpecAsync(new GameByIdSpec(basketItem.Game.Id))
                   ?? throw new ItemNotFoundException(typeof(Game), basketItem.Game.Id, nameof(basketItem.Game.Id));

        orderDetails.GameId = game.Id;
        orderDetails.Game = game;
        orderDetails.Discount = DefaultDiscount;
        orderDetails.Quantity = basketItem.Quantity;

        var unitPrice = basketItem.Quantity * game.Price - orderDetails.Discount;
        orderDetails.Price = unitPrice;

        return orderDetails;
    }
}