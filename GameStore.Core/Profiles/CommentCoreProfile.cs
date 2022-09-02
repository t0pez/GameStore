using AutoMapper;
using GameStore.Core.Models.Server.Comments;
using GameStore.Core.Models.ServiceModels.Comments;

namespace GameStore.Core.Profiles;

public class CommentCoreProfile : Profile
{
    public CommentCoreProfile()
    {
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