using GameStore.Core.Models.Server.Orders;

namespace GameStore.Core.Helpers.PdfGenerators;

public interface IInvoiceFileGenerator
{
    public byte[] GetFile(Order order);
}