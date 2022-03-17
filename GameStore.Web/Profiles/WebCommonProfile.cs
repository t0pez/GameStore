using AutoMapper;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Records;
using GameStore.Web.Models;
using GameStore.Web.ViewModels;

namespace GameStore.Web.Profiles;

public class WebCommonProfile : Profile
{
    public WebCommonProfile()
    {
        CreateMap<GameCreateRequestModel, GameCreateModel>().ReverseMap();
        CreateMap<GameEditRequestModel, GameUpdateModel>().ReverseMap();
        CreateMap<CommentCreateRequestModel, CommentCreateModel>().ReverseMap();
        CreateMap<ReplyCreateRequestModel, ReplyCreateModel>().ReverseMap();

        CreateMap<Game, GameViewModel>();
        CreateMap<Genre, GenreViewModel>();
        CreateMap<PlatformType, PlatformTypeViewModel>();
        CreateMap<Comment, CommentViewModel>()
            .ForMember(model => model.GameKey,
                expression => expression.MapFrom(comment => comment.Game.Key))
            .ForMember(model => model.AuthorName,
                expression => expression.MapFrom(comment => comment.Name));
    }
}