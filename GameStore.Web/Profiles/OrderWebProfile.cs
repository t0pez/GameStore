using AutoMapper;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.ServiceModels.Orders;
using GameStore.Web.Models.Order;
using GameStore.Web.ViewModels.Order;

namespace GameStore.Web.Profiles;

public class OrderWebProfile : Profile
{
    public OrderWebProfile()
    {
        CreateMap<Order, OrderViewModel>();
        CreateMap<Order, OrderListViewModel>();
        
        CreateMap<OrderDto, OrderViewModel>();
        CreateMap<OrderDto, OrderListViewModel>();

        CreateMap<OrderDetails, OrderDetailsViewModel>();
        CreateMap<OrderDetailsDto, OrderDetailsViewModel>();

        CreateMap<BasketItem, OrderDetails>();

        CreateMap<ActiveOrderCreateRequestModel, ActiveOrderCreateModel>();

        CreateMap<AllOrdersFilterRequestModel, AllOrdersFilter>();
    }
}