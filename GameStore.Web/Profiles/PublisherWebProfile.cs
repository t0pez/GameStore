using AutoMapper;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Server.Publishers;
using GameStore.Core.Models.ServiceModels.Publishers;
using GameStore.Web.Models.Publisher;
using GameStore.Web.ViewModels.Publisher;

namespace GameStore.Web.Profiles;

public class PublisherWebProfile : Profile
{
    public PublisherWebProfile()
    {
        CreateMap<PublisherCreateRequestModel, PublisherCreateModel>().ReverseMap();
        CreateMap<PublisherUpdateRequestModel, PublisherUpdateModel>().ReverseMap();

        CreateMap<Publisher, PublisherViewModel>();
        CreateMap<Publisher, PublisherListViewModel>();
        CreateMap<Publisher, PublisherInGameViewModel>();
        CreateMap<Publisher, PublisherUpdateRequestModel>();

        CreateMap<PublisherDto, PublisherViewModel>().ReverseMap();
        CreateMap<PublisherDto, PublisherListViewModel>().ReverseMap();
        CreateMap<PublisherDto, PublisherInGameViewModel>().ReverseMap();
        CreateMap<PublisherDto, PublisherUpdateRequestModel>().ReverseMap();
    }
}