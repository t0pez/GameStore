namespace GameStore.Web.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool IsShowRequestId => string.IsNullOrEmpty(RequestId) == false;
}
