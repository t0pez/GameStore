using GameStore.Core.Interfaces.PaymentMethods;

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

public class VisaPaymentGetaway : PaymentGetaway
{
    public decimal TotalSum { get; set; }
}