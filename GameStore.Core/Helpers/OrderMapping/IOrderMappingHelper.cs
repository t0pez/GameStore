using System.Threading.Tasks;
using GameStore.Core.Models.Baskets;
using GameStore.Core.Models.Orders;

namespace GameStore.Core.Helpers.OrderMapping;

public interface IOrderMappingHelper
{
    public Task<Order> GetOrderAsync(Basket basket);
}