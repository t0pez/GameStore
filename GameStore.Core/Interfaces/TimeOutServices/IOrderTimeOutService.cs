using System;
using System.Threading.Tasks;
using GameStore.Core.Models.Server.Orders;

namespace GameStore.Core.Interfaces.TimeOutServices;

public interface IOrderTimeOutService
{
    public Task CreateOpenedOrderAsync(Order order);
    public Task RemoveOpenedOrderByOrderIdAsync(Guid orderId);
}