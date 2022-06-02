using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.ServiceModels.Orders;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Quartz;

namespace GameStore.Core.Services.HostedServices;

public class OrderTimeOutJob : IJob
{
    private readonly IOrderService _orderService;
    private readonly IOrderTimeOutService _orderTimeOutService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderTimeOutJob(IOrderTimeOutService orderTimeOutService, IUnitOfWork unitOfWork, IOrderService orderService, IMapper mapper)
    {
        _orderTimeOutService = orderTimeOutService;
        _unitOfWork = unitOfWork;
        _orderService = orderService;
        _mapper = mapper;
    }

    private IRepository<OpenedOrder> OpenedOrderRepository => _unitOfWork.GetRepository<OpenedOrder>();

    public async Task Execute(IJobExecutionContext context)
    {
        var openedOrders = await OpenedOrderRepository.GetBySpecAsync();

        foreach (var openedOrder in openedOrders.Where(openedOrder => openedOrder.TimeOutDate < DateTime.UtcNow))
        {
            await _orderTimeOutService.RemoveOpenedOrderByOrderIdAsync(openedOrder.OrderId);
            await SetCancelledOrderStatus(openedOrder.OrderId);
        }
    }

    private async Task SetCancelledOrderStatus(Guid orderId)
    {
        var order = await _orderService.GetByIdAsync(orderId);
        order.Status = OrderStatus.Cancelled;

        var updateModel = _mapper.Map<OrderUpdateModel>(order);
        await _orderService.UpdateAsync(updateModel);
    }
}