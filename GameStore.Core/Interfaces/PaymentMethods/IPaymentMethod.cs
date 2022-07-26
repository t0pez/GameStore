using GameStore.Core.Models.Payment;
using GameStore.Core.Models.ServiceModels.Payment;

namespace GameStore.Core.Interfaces.PaymentMethods;

public interface IPaymentMethod
{
    public PaymentGetaway GetPaymentGetaway(PaymentGetawayCreateModel createModel);
}