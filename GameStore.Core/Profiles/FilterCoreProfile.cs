using AutoMapper;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Core.Models.Mongo.Products.Filters;
using GameStore.Core.Models.Server.Games.Filters;

namespace GameStore.Core.Profiles;

public class FilterCoreProfile : Profile
{
    public FilterCoreProfile()
    {
        CreateMap<AllProductsFilter, GameSearchFilter>();
        CreateMap<AllProductsFilter, ProductFilter>();
    }
}