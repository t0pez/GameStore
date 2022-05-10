using System;

namespace GameStore.Core.Interfaces.PaymentMethods;

public interface IPaymentMethod
{
    public PaymentGetaway GetPaymentGetaway(PaymentGetawayCreateModel createModel);
}

public class PaymentGetawayCreateModel
{
    public Guid OrderId { get; set; }
    public decimal TotalSum { get; set; }
}

public abstract class PaymentGetaway
{
    
}
