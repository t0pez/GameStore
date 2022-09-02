using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Core.Models.Server.Orders;
using GameStore.Core.Models.ServiceModels.Orders;

namespace GameStore.Core.Interfaces;

public interface IOrderService
{
    public Task<ICollection<OrderDto>> GetByFilterAsync(AllOrdersFilter filter);
    public Task<Order> GetByIdAsync(Guid orderId);
    public Task<Order> GetBasketByCustomerIdAsync(Guid customerId);
    public Task<ICollection<Order>> GetByCustomerIdAsync(Guid customerId);
    public Task<Order> CreateAsync(OrderCreateModel createModel);
    public Task MakeOrder(Guid orderId);
    public Task FillShippersAsync(ActiveOrderCreateModel createModel);
    public Task AddToOrderAsync(Guid customerId, BasketItem item);
    public Task UpdateAsync(OrderUpdateModel updateModel);
    public Task<bool> IsCustomerHasActiveOrderAsync(Guid customerId);
    public Task<bool> IsCustomerHasBasketAsync(Guid customerId);
}