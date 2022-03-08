using AutoMapper;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Records;
using GameStore.Web.Models;
using GameStore.Web.ViewModels;

namespace GameStore.Web.Profiles
{
    public class CommonProfile : Profile
    {
        public CommonProfile()
        {
            CreateMap<GameCreateRequestModel, GameCreateModel>().ReverseMap();
            CreateMap<CommentCreateRequestModel, CommentCreateModel>().ReverseMap();
            CreateMap<GameEditRequestModel, GameUpdateModel>().ReverseMap();

            CreateMap<Genre, GenreViewModel>().ReverseMap();
            CreateMap<PlatformType, PlatformTypeViewModel>().ReverseMap();
        }
    }
}
