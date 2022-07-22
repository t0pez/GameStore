using System.Linq;
using AutoMapper;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.Web.Models.Game;
using GameStore.Web.ViewModels.Games;

namespace GameStore.Web.Profiles;

public class GameWebProfile : Profile
{
    public GameWebProfile()
    {
        CreateMap<GameCreateRequestModel, GameCreateModel>().ReverseMap();
        CreateMap<GameUpdateRequestModel, GameUpdateModel>().ReverseMap();
        
        CreateMap<Game, GameUpdateModel>()
            .ForMember(updateModel => updateModel.GenresIds,
                       expression => expression.MapFrom(game => game.Genres.Select(gg => gg.GenreId)))
            .ForMember(updateModel => updateModel.PlatformsIds,
                       expression => expression.MapFrom(game => game.Platforms.Select(gp => gp.PlatformId)));
        
        CreateMap<Game, GameViewModel>();
        CreateMap<Game, GameListViewModel>();
        CreateMap<Game, GameInBasketViewModel>();
        CreateMap<Game, GameInOrderDetailsViewModel>();
        CreateMap<Game, GameUpdateRequestModel>();
        
        CreateMap<ProductDto, GameViewModel>();
        CreateMap<ProductDto, GameInOrderDetailsViewModel>();
        CreateMap<ProductDto, GameUpdateRequestModel>();

        CreateMap<ProductDto, GameUpdateModel>()
            .ForMember(model => model.GenresIds,
                       expression => expression.MapFrom(dto => dto.Genres.Select(genre => genre.Id)))
            .ForMember(model => model.PlatformsIds,
                       expression => expression.MapFrom(dto => dto.Platforms.Select(genre => genre.Id)));
    }
}