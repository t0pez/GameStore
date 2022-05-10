using System;
using GameStore.Core.Interfaces.PaymentMethods;

namespace GameStore.Core.Services.PaymentMethods;

public class IboxPaymentMethod : IPaymentMethod
{
    public PaymentGetaway GetPaymentGetaway(PaymentGetawayCreateModel createModel)
    {
        var getaway = new IboxPaymentGetaway
        {
            OrderId = createModel.OrderId,
            TotalSum = createModel.TotalSum
        };

        return getaway;
    }
}

public class IboxPaymentGetaway : PaymentGetaway
{
    public Guid OrderId { get; set; }
    public decimal TotalSum { get; set; }
}