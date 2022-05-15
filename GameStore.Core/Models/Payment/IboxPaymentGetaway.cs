using System;

namespace GameStore.Core.Models.Payment;

public class IboxPaymentGetaway : PaymentGetaway
{
    public Guid OrderId { get; set; }
    public decimal TotalSum { get; set; }
}