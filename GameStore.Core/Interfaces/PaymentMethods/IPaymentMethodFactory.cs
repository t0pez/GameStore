using System;
using GameStore.Core.Helpers.PdfGenerators;
using GameStore.Core.Services.PaymentMethods;

namespace GameStore.Core.Interfaces.PaymentMethods;

public interface IPaymentMethodFactory
{
    public IPaymentMethod GetPaymentMethod(PaymentType type);
}

public class PaymentMethodFactory : IPaymentMethodFactory
{
    private readonly IInvoiceFileGenerator _invoiceFileGenerator;

    public PaymentMethodFactory(IInvoiceFileGenerator invoiceFileGenerator)
    {
        _invoiceFileGenerator = invoiceFileGenerator;
    }

    public IPaymentMethod GetPaymentMethod(PaymentType type)
    {
        return type switch
        {
            PaymentType.Visa => new VisaPaymentMethod(),
            PaymentType.Ibox => new IboxPaymentMethod(),
            PaymentType.Bank => new BankPaymentMethod(_invoiceFileGenerator),
            _                => throw new ArgumentOutOfRangeException(nameof(type), type, "Wrong payment method")
        };
    }
}

public enum PaymentType
{
    Visa,
    Ibox,
    Bank
}