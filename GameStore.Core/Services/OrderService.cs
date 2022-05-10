using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Core.Helpers.OrderMapping;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.Orders.Specifications;
using GameStore.Core.Models.ServiceModels.Orders;
using GameStore.SharedKernel.Interfaces.DataAccess;

namespace GameStore.Core.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderMappingHelper _orderMappingHelper;

    public OrderService(IUnitOfWork unitOfWork, IOrderMappingHelper orderMappingHelper)
    {
        _unitOfWork = unitOfWork;
        _orderMappingHelper = orderMappingHelper;
    }

    private IRepository<Order> OrderRepository => _unitOfWork.GetRepository<Order>();

    public async Task<ICollection<Order>> GetAllAsync()
    {
        var result = await OrderRepository.GetBySpecAsync(new OrdersListSpec());

        return result;
    }

    public async Task<Order> GetByIdAsync(Guid id)
    {
        var result = await OrderRepository.GetSingleOrDefaultBySpecAsync(new OrderByIdWithDetailsSpec(id));

        return result;
    }

    public async Task<ICollection<Order>> GetByCustomerIdAsync(Guid customerId)
    {
        var result = await OrderRepository.GetBySpecAsync(new OrdersByCustomerIdSpec(customerId));

        return result;
    }
    
    public async Task<Order> CreateAsync(OrderCreateModel createModel)
    {
        var order = await _orderMappingHelper.GetOrderAsync(createModel.Basket);
        
        order.OrderDate = DateTime.UtcNow;
        order.Status = OrderStatus.Created;
        
        await OrderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        return order;
    }

    public async Task UpdateAsync(OrderUpdateModel updateModel)
    {
        var order = await OrderRepository.GetSingleOrDefaultBySpecAsync(new OrderByIdWithDetailsSpec(updateModel.Id));

        UpdateValues(order, updateModel);
    }

    public async Task DeleteAsync(Guid id)
    {
        var order = await OrderRepository.GetSingleOrDefaultBySpecAsync(new OrderByIdSpec(id));

        order.IsDeleted = true;
        await OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
    }

    private void UpdateValues(Order order, OrderUpdateModel updateModel)
    {
        order.CustomerId = updateModel.CustomerId;
        order.OrderDetails = updateModel.OrderDetails;
        order.OrderDate = updateModel.OrderDate;
        order.Status = updateModel.Status;
    }
}