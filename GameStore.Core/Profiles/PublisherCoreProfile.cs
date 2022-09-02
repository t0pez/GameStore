using AutoMapper;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Mongo.Suppliers;
using GameStore.Core.Models.Server.Publishers;
using GameStore.Core.Models.ServiceModels.Publishers;

namespace GameStore.Core.Profiles;

public class PublisherCoreProfile : Profile
{
    public PublisherCoreProfile()
    {
        CreateMap<PublisherCreateModel, Publisher>();

        CreateMap<Publisher, PublisherDto>();

        CreateMap<Supplier, PublisherDto>()
            .ForMember(dto => dto.Id, expression => expression.Ignore())
            .ForMember(dto => dto.Name,
                       expression => expression.MapFrom(supplier => supplier.CompanyName));
    }
}