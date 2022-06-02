using System;
using System.Threading.Tasks;
using GameStore.Core.Models.Orders;

namespace GameStore.Core.Interfaces;

public interface IOpenedOrderService
{
    public Task CreateAsync(OpenedOrder openedOrder);
    public Task DeleteByOrderIdAsync(Guid oderId);
}