using AutoMapper;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.ServiceModels.Comments;
using GameStore.Core.Models.ServiceModels.Games;

namespace GameStore.Core.Profiles;

public class CoreCommonProfile : Profile
{
    public CoreCommonProfile()
    {
        CreateMap<GameCreateModel, Game>();
        CreateMap<CommentCreateModel, Comment>()
            .ForMember(comment => comment.Body,
                expression => expression.MapFrom(model => model.Message))
            .ForMember(comment => comment.Name,
                expression => expression.MapFrom(model => model.AuthorName));
        CreateMap<ReplyCreateModel, Comment>()
            .ForMember(comment => comment.Body,
                expression => expression.MapFrom(model => model.Message))
            .ForMember(comment => comment.Name,
                expression => expression.MapFrom(model => model.AuthorName));
    }
}