using System;
using System.Threading.Tasks;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.PaymentMethods;
using GameStore.Core.Models.Orders;

namespace GameStore.Core.Services;

public class PaymentService : IPaymentService
{
    private readonly IOrderService _orderService;
    private IPaymentMethod _paymentMethod;

    public PaymentService(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<object> GetPaymentGateway(Order order)
    {
        var paymentResult = _paymentMethod.GetPaymentGetaway(order);

        order.Status = OrderStatus.Pending;

        await _orderService.CreateAsync(order);
        
        return paymentResult;
    }

    public void SetPaymentMethod(IPaymentMethod paymentMethod)
    {
        _paymentMethod = paymentMethod ?? throw new ArgumentNullException(nameof(paymentMethod));
    }
}