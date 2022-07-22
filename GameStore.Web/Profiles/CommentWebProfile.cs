using AutoMapper;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.ServiceModels.Comments;
using GameStore.Web.Models.Comment;
using GameStore.Web.ViewModels.Comments;

namespace GameStore.Web.Profiles;

public class CommentWebProfile : Profile
{
    public CommentWebProfile()
    {
        CreateMap<CommentCreateRequestModel, CommentCreateModel>().ReverseMap();
        CreateMap<CommentCreateRequestModel, ReplyCreateModel>().ReverseMap();
        CreateMap<CommentUpdateRequestModel, CommentUpdateModel>().ReverseMap();
        
        CreateMap<Comment, CommentViewModel>()
            .ForMember(model => model.GameKey,
                       expression => expression.MapFrom(comment => comment.Game.Key))
            .ForMember(model => model.AuthorName,
                       expression => expression.MapFrom(comment => comment.Name))
            .ForMember(model => model.MessageIsDeleted,
                       expression => expression.MapFrom(comment => comment.IsDeleted));
        CreateMap<Comment, CommentInReplyViewModel>()
            .ForMember(model => model.AuthorName,
                       expression => expression.MapFrom(comment => comment.Name))
            .ForMember(model => model.MessageIsDeleted,
                       expression => expression.MapFrom(comment => comment.IsDeleted));
    }
}