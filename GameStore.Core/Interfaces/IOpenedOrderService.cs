using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Core.Models.Orders;

namespace GameStore.Core.Interfaces;

public interface IOpenedOrderService
{
    public Task<IEnumerable<OpenedOrder>> GetAll();
    public Task CreateAsync(OpenedOrder openedOrder);
    public Task DeleteByOrderIdAsync(Guid oderId);
}