using System.Threading.Tasks;
using GameStore.Core.Interfaces.PaymentMethods;
using GameStore.Core.Models.Orders;

namespace GameStore.Core.Interfaces;

public interface IPaymentService
{
    public Task<object> GetPaymentGateway(Order order);
    public void SetPaymentMethod(IPaymentMethod paymentMethod);
}