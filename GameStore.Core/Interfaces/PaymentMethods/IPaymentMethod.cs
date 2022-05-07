using GameStore.Core.Models.Orders;

namespace GameStore.Core.Interfaces.PaymentMethods;

public interface IPaymentMethod
{
    public object GetPaymentGetaway(Order order);
    public object EnrollPayment(Order order);
}
