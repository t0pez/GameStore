using GameStore.Core.Interfaces.PaymentMethods;
using GameStore.Core.Models.Payment;
using GameStore.Core.Models.ServiceModels.Payment;

namespace GameStore.Core.Services.PaymentMethods;

public class VisaPaymentMethod : IPaymentMethod
{
    public PaymentGetaway GetPaymentGetaway(PaymentGetawayCreateModel createModel)
    {
        var paymentGetaway = new VisaPaymentGetaway
        {
            TotalSum = createModel.Order.TotalSum
        };
        
        return paymentGetaway;
    }
}