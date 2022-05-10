using System;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.PaymentMethods;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.Payment;
using GameStore.Core.Models.ServiceModels.Orders;
using GameStore.Core.Models.ServiceModels.Payment;

namespace GameStore.Core.Services;

public class PaymentService : IPaymentService
{
    private readonly IOrderService _orderService;
    private readonly IPaymentMethodFactory _paymentMethodFactory;
    private readonly IMapper _mapper;
    private IPaymentMethod _paymentMethod;

    public PaymentService(IOrderService orderService, IPaymentMethodFactory paymentMethodFactory, IMapper mapper)
    {
        _orderService = orderService;
        _paymentMethodFactory = paymentMethodFactory;
        _mapper = mapper;
    }

    public async Task<PaymentGetaway> GetPaymentGateway(Order order, PaymentType? paymentType = null)
    {
        if (paymentType is not null)
        {
            var paymentMethod = _paymentMethodFactory.GetPaymentMethod((PaymentType)paymentType);
            SetPaymentMethod(paymentMethod);
        }

        var getawayCreateModel = new PaymentGetawayCreateModel
        {
            Order = order
        };
        
        var paymentResult = _paymentMethod.GetPaymentGetaway(getawayCreateModel);

        order.Status = OrderStatus.Pending;
        
        var updateModel = _mapper.Map<OrderUpdateModel>(order);
        await _orderService.UpdateAsync(updateModel);

        return paymentResult;
    }

    public void SetPaymentMethod(IPaymentMethod paymentMethod)
    {
        _paymentMethod = paymentMethod ?? throw new ArgumentNullException(nameof(paymentMethod));
    }
}