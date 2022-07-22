using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.ServiceModels.Orders;

namespace GameStore.Core.Interfaces;

public interface IOrderService
{
    public Task<ICollection<OrderDto>> GetByFilterAsync(AllOrdersFilter filter);
    public Task<Order> GetByIdAsync(Guid orderId);
    public Task<Order> GetBasketByCustomerIdAsync(string customerId);
    public Task<ICollection<Order>> GetByCustomerIdAsync(string customerId);
    public Task<Order> CreateAsync(OrderCreateModel createModel);
    public Task MakeOrder(Guid orderId);
    public Task FillShippersAsync(ActiveOrderCreateModel createModel);
    public Task AddToOrderAsync(string customerId, BasketItem item);
    public Task UpdateAsync(OrderUpdateModel updateModel);
    public Task<bool> IsCustomerHasActiveOrder(string customerId);
}