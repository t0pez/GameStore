using System.Text;
using GameStore.Core.Models.Server.Orders;
using SelectPdf;

namespace GameStore.Core.Helpers.PdfGenerators;

public class InvoiceFileGenerator : IInvoiceFileGenerator
{
    public byte[] GetFile(Order order)
    {
        var converter = new HtmlToPdf();

        var orderHtml = GetOrderHtml(order);

        var pdf = converter.ConvertHtmlString(orderHtml);
        var pdfBytes = pdf.Save();

        return pdfBytes;
    }

    private string GetOrderHtml(Order order)
    {
        var stringBuilder = new StringBuilder();

        AddTableWithOrderDetails(stringBuilder, order);
        AddTotalSum(stringBuilder, order);

        return stringBuilder.ToString();
    }

    private void AddTableWithOrderDetails(StringBuilder stringBuilder, Order order)
    {
        stringBuilder.Append("<table>");

        const string tableHeader = "<tr>" +
                                   "<th>Product name</th>" +
                                   "<th>Product quantity</th>" +
                                   "<th>Product price</th>" +
                                   "</tr>";

        stringBuilder.Append(tableHeader);

        foreach (var orderDetail in order.OrderDetails)
        {
            var orderTableRow =
                "<tr>" +
                $"<td>{orderDetail.Game.Name}</td>" +
                $"<td>{orderDetail.Quantity}</td>" +
                $"<td>{orderDetail.Price}</td>" +
                "</tr>";

            stringBuilder.Append(orderTableRow);
        }

        stringBuilder.Append("</table>");
    }

    private void AddTotalSum(StringBuilder stringBuilder, Order order)
    {
        stringBuilder.Append($"<h4>Total sum: {order.TotalSum}</h4>");
    }
}