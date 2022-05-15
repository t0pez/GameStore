namespace GameStore.Core.Models.Payment;

public class BankPaymentGetaway : PaymentGetaway
{
    public byte[] InvoiceFileContent { get; set; }
}