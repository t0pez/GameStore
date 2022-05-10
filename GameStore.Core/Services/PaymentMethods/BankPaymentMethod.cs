using GameStore.Core.Interfaces.PaymentMethods;
using Newtonsoft.Json;

namespace GameStore.Core.Services.PaymentMethods;

public class BankPaymentMethod : IPaymentMethod
{
    public PaymentGetaway GetPaymentGetaway(PaymentGetawayCreateModel createModel)
    {
        var getaway = new BankPaymentGetaway();

        var orderJson = JsonConvert.SerializeObject(createModel);
        getaway.InvoiceFileContent = orderJson;
        
        return getaway;
    }
}

public class BankPaymentGetaway : PaymentGetaway
{
    public string InvoiceFileContent { get; set; }
}