using GameStore.Core.Interfaces.PaymentMethods;
using GameStore.Core.Models.Payment;
using GameStore.Core.Models.ServiceModels.Payment;

namespace GameStore.Core.Services.PaymentMethods;

public class IboxPaymentMethod : IPaymentMethod
{
    public PaymentGetaway GetPaymentGetaway(PaymentGetawayCreateModel createModel)
    {
        var getaway = new IboxPaymentGetaway
        {
            OrderId = createModel.Order.Id,
            TotalSum = createModel.Order.TotalSum
        };

        return getaway;
    }
}