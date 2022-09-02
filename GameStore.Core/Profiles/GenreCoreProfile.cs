using AutoMapper;
using GameStore.Core.Models.Mongo.Categories;
using GameStore.Core.Models.Server.Genres;
using GameStore.Core.Models.ServiceModels.Genres;

namespace GameStore.Core.Profiles;

public class GenreCoreProfile : Profile
{
    public GenreCoreProfile()
    {
        CreateMap<Category, Genre>().ReverseMap();

        CreateMap<GenreCreateModel, Genre>();
    }
}