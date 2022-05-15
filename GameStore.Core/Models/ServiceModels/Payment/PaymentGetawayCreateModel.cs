using GameStore.Core.Models.Orders;

namespace GameStore.Core.Models.ServiceModels.Payment;

public class PaymentGetawayCreateModel
{
    public Order Order { get; set; }
}