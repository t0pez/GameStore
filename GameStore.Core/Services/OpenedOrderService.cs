using System;
using System.Threading.Tasks;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.Orders.Specifications;
using GameStore.SharedKernel.Interfaces.DataAccess;

namespace GameStore.Core.Services;

public class OpenedOrderService : IOpenedOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OpenedOrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private IRepository<OpenedOrder> OpenedOrderRepository => _unitOfWork.GetRepository<OpenedOrder>();

    public async Task CreateAsync(OpenedOrder openedOrder)
    {
        await OpenedOrderRepository.AddAsync(openedOrder);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteByOrderIdAsync(Guid orderId)
    {
        var openedOrder = await OpenedOrderRepository.GetSingleOrDefaultBySpecAsync(new OpenedOrderByOrderIdSpec(orderId))
                          ?? throw new ItemNotFoundException(typeof(OpenedOrder), orderId);

        await OpenedOrderRepository.DeleteAsync(openedOrder);
        await _unitOfWork.SaveChangesAsync();
    }
}