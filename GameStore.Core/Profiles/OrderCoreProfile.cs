using AutoMapper;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Core.Models.Mongo.Orders;
using GameStore.Core.Models.Mongo.Orders.Filters;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.Orders.Filters;
using GameStore.Core.Models.ServiceModels.Orders;

namespace GameStore.Core.Profiles;

public class OrderCoreProfile : Profile
{
    public OrderCoreProfile()
    {
        CreateMap<Order, OrderUpdateModel>();
        CreateMap<OrderCreateModel, Order>();

        CreateMap<OrderMongo, Order>()
            .ForMember(order => order.Id,
                       expression => expression.Ignore());
        CreateMap<OrderDetailsMongo, OrderDetails>()
            .ForMember(orderDetails => orderDetails.Id,
                       expression => expression.Ignore())
            .ForMember(orderDetails => orderDetails.OrderId,
                       expression => expression.Ignore());

        CreateMap<OrderMongo, OrderDto>();
        CreateMap<OrderDetailsMongo, OrderDetailsDto>()
            .ForMember(dto => dto.TotalPrice,
                       expression => expression.MapFrom(mongo => mongo.TotalPrice));

        CreateMap<Order, OrderDto>();
        CreateMap<OrderDetails, OrderDetailsDto>()
            .ForMember(dto => dto.TotalPrice,
                       expression => expression.MapFrom(mongo => mongo.TotalPrice));

        CreateMap<AllOrdersFilter, OrdersFilter>();
        CreateMap<AllOrdersFilter, MongoOrdersFilter>();
    }
}