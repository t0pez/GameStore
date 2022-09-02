using System.Collections.Generic;
using GameStore.Web.Models.Order;

namespace GameStore.Web.ViewModels.Order;

public class GetAllViewModel
{
    public IEnumerable<OrderListViewModel> Orders { get; set; }

    public AllOrdersFilterRequestModel Filter { get; set; }
}