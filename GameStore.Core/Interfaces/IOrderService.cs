using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.ServiceModels.Orders;

namespace GameStore.Core.Interfaces;

public interface IOrderService
{
    public Task<ICollection<Order>> GetAllAsync();
    public Task<Order> GetByIdAsync(Guid id);
    public Task<ICollection<Order>> GetByCustomerIdAsync(Guid customerId);
    public Task<Order> CreateAsync(OrderCreateModel createModel);
    public Task UpdateAsync(OrderUpdateModel updateModel);
    public Task DeleteAsync(Guid id);
}