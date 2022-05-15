using GameStore.Core.Helpers.PdfGenerators;
using GameStore.Core.Interfaces.PaymentMethods;
using GameStore.Core.Models.Payment;
using GameStore.Core.Models.ServiceModels.Payment;

namespace GameStore.Core.Services.PaymentMethods;

public class BankPaymentMethod : IPaymentMethod
{
    private readonly IInvoiceFileGenerator _invoiceFileGenerator;

    public BankPaymentMethod(IInvoiceFileGenerator invoiceFileGenerator)
    {
        _invoiceFileGenerator = invoiceFileGenerator;
    }

    public PaymentGetaway GetPaymentGetaway(PaymentGetawayCreateModel createModel)
    {
        var getaway = new BankPaymentGetaway();

        var fileContent = _invoiceFileGenerator.GetFile(createModel.Order);
        getaway.InvoiceFileContent = fileContent;
        
        return getaway;
    }
}

