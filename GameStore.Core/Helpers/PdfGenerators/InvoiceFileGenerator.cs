using System;
using System.Text;
using GameStore.Core.Models.Orders;
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

        return stringBuilder.ToString();
    }
}