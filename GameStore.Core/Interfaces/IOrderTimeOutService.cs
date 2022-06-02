using System;
using System.Threading.Tasks;
using GameStore.Core.Models.Orders;

namespace GameStore.Core.Interfaces;

public interface IOrderTimeOutService
{
    public Task CreateOpenedOrderAsync(Order order);
    public Task RemoveOpenedOrderByOrderIdAsync(Guid orderId);
}