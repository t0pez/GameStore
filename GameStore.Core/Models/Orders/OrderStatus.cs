using System.ComponentModel.DataAnnotations;

namespace GameStore.Core.Models.Orders;

public enum OrderStatus
{
    [Display(Name = "Created")] Created,
    [Display(Name = "Pending")] InProcess,
    [Display(Name = "Cancelled")] Cancelled,
    [Display(Name = "Completed")] Completed
}