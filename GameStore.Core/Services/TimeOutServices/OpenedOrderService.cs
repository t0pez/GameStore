using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces.TimeOutServices;
using GameStore.Core.Models.Server.Orders;
using GameStore.Core.Models.Server.Orders.Specifications;
using GameStore.SharedKernel.Interfaces.DataAccess;

namespace GameStore.Core.Services.TimeOutServices;

public class OpenedOrderService : IOpenedOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OpenedOrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private IRepository<OpenedOrder> OpenedOrderRepository => _unitOfWork.GetEfRepository<OpenedOrder>();

    public async Task<IEnumerable<OpenedOrder>> GetAllAsync()
    {
        var result = await OpenedOrderRepository.GetBySpecAsync();

        return result;
    }

    public async Task CreateAsync(OpenedOrder openedOrder)
    {
        await OpenedOrderRepository.AddAsync(openedOrder);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(OpenedOrder updated)
    {
        var spec = new OpenedOrdersSpec().ByOrderId(updated.OrderId);

        var openedOrder = await OpenedOrderRepository.GetSingleOrDefaultBySpecAsync(spec)
                          ?? throw new ItemNotFoundException(typeof(OpenedOrder), updated.OrderId,
                                                             nameof(updated.OrderId));

        openedOrder.TimeOutDate = updated.TimeOutDate;

        await OpenedOrderRepository.UpdateAsync(openedOrder);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteByOrderIdAsync(Guid orderId)
    {
        var spec = new OpenedOrdersSpec().ByOrderId(orderId);

        var openedOrder = await OpenedOrderRepository.GetSingleOrDefaultBySpecAsync(spec)
                          ?? throw new ItemNotFoundException(typeof(OpenedOrder), orderId);

        await OpenedOrderRepository.DeleteAsync(openedOrder);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> IsOrderExistsAsync(Guid orderId)
    {
        var spec = new OpenedOrdersSpec().ByOrderId(orderId);

        return await OpenedOrderRepository.AnyAsync(spec);
    }
}