using AutoMapper;
using GameStore.Core.Models.Server.Genres;
using GameStore.Core.Models.Server.RelationalModels;
using GameStore.Core.Models.ServiceModels.Genres;
using GameStore.Web.Models.Genre;
using GameStore.Web.ViewModels.Genres;

namespace GameStore.Web.Profiles;

public class GenreWebProfile : Profile
{
    public GenreWebProfile()
    {
        CreateMap<GenreCreateRequestModel, GenreCreateModel>().ReverseMap();
        CreateMap<GenreUpdateRequestModel, GenreUpdateModel>().ReverseMap();

        CreateMap<Genre, GenreViewModel>();
        CreateMap<Genre, GenreListViewModel>();

        CreateMap<GameGenre, GenreViewModel>()
            .ForMember(viewModel => viewModel.Id,
                       expression => expression.MapFrom(gameGenre => gameGenre.GenreId))
            .ForMember(viewModel => viewModel.Name,
                       expression => expression.MapFrom(gameGenre => gameGenre.Genre.Name))
            .ForMember(viewModel => viewModel.SubGenres,
                       expression => expression.MapFrom(gameGenre => gameGenre.Genre.SubGenres));

        CreateMap<GameGenre, GenreListViewModel>()
            .ForMember(viewModel => viewModel.Id,
                       expression => expression.MapFrom(gameGenre => gameGenre.GenreId))
            .ForMember(viewModel => viewModel.Name,
                       expression => expression.MapFrom(gameGenre => gameGenre.Genre.Name));
    }
}