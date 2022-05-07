using System;
using GameStore.Core.Interfaces.PaymentMethods;
using GameStore.Core.Models.Orders;

namespace GameStore.Core.Services.PaymentMethods;

public class VisaPaymentMethod : IPaymentMethod
{
    private readonly IThirdPartyPaymentApiService _api;

    public object GetPaymentGetaway(Order order)
    {
        _api?.Foo();
        
        var paymentGetaway = new VisaPaymentGetaway
        {
            OrderId = order.Id, 
            TotalSum = order.TotalSum, 
            PaymentUrl = "Some url"
        };
        
        return paymentGetaway;
    }

    public object EnrollPayment(Order order)
    {
        throw new NotImplementedException();
    }
}

public class VisaPaymentGetaway
{
    public Guid OrderId { get; set; }
    public decimal TotalSum { get; set; }
    public string PaymentUrl { get; set; }
}

public interface IThirdPartyPaymentApiService
{
    void Foo();
}