using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.TimeOutServices;
using GameStore.Core.Models.Server.Orders;
using GameStore.Core.Models.ServiceModels.Orders;
using Quartz;

namespace GameStore.Core.Services.HostedServices;

public class OrderTimeOutJob : IJob
{
    private readonly IMapper _mapper;
    private readonly IOpenedOrderService _openedOrderService;
    private readonly IOrderService _orderService;
    private readonly IOrderTimeOutService _orderTimeOutService;

    public OrderTimeOutJob(IOrderTimeOutService orderTimeOutService, IOpenedOrderService openedOrderService,
                           IOrderService orderService, IMapper mapper)
    {
        _orderTimeOutService = orderTimeOutService;
        _openedOrderService = openedOrderService;
        _orderService = orderService;
        _mapper = mapper;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var openedOrders = await _openedOrderService.GetAllAsync();

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