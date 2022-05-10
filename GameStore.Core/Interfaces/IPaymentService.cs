using System.Threading.Tasks;
using GameStore.Core.Interfaces.PaymentMethods;
using GameStore.Core.Models.Orders;

namespace GameStore.Core.Interfaces;

public interface IPaymentService
{
    public Task<PaymentGetaway> GetPaymentGateway(Order order, PaymentType? paymentType = null);
    public void SetPaymentMethod(IPaymentMethod paymentMethod);
}