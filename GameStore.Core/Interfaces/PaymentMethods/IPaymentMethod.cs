using GameStore.Core.Models.Orders;

namespace GameStore.Core.Interfaces.PaymentMethods;

public interface IPaymentMethod
{
    public PaymentGetaway GetPaymentGetaway(PaymentGetawayCreateModel createModel);
}

public class PaymentGetawayCreateModel
{
    public Order Order { get; set; }
}

public abstract class PaymentGetaway
{
    
}
