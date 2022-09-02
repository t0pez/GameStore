using AutoMapper;
using GameStore.Core.Models.Server.PlatformTypes;
using GameStore.Core.Models.Server.RelationalModels;
using GameStore.Core.Models.ServiceModels.PlatformTypes;
using GameStore.Web.Models.PlatformType;
using GameStore.Web.ViewModels.PlatformTypes;

namespace GameStore.Web.Profiles;

public class PlatformTypeWebProfile : Profile
{
    public PlatformTypeWebProfile()
    {
        CreateMap<PlatformTypeCreateRequestModel, PlatformTypeCreateModel>().ReverseMap();
        CreateMap<PlatformTypeUpdateRequestModel, PlatformTypeUpdateModel>().ReverseMap();

        CreateMap<PlatformType, PlatformTypeViewModel>();
        CreateMap<PlatformType, PlatformTypeListViewModel>();

        CreateMap<GamePlatformType, PlatformTypeViewModel>()
            .ForMember(viewModel => viewModel.Id,
                       expression => expression.MapFrom(gameGenre => gameGenre.PlatformId))
            .ForMember(viewModel => viewModel.Name,
                       expression => expression.MapFrom(gameGenre => gameGenre.Platform.Name));

        CreateMap<GamePlatformType, PlatformTypeListViewModel>()
            .ForMember(viewModel => viewModel.Id,
                       expression => expression.MapFrom(gameGenre => gameGenre.PlatformId))
            .ForMember(viewModel => viewModel.Name,
                       expression => expression.MapFrom(gameGenre => gameGenre.Platform.Name));
    }
}