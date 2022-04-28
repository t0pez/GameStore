using System.Linq;
using AutoMapper;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Genres;
using GameStore.Core.Models.PlatformTypes;
using GameStore.Core.Models.Publishers;
using GameStore.Core.Models.RelationalModels;
using GameStore.Core.Models.ServiceModels.Comments;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.Core.Models.ServiceModels.Genres;
using GameStore.Core.Models.ServiceModels.PlatformTypes;
using GameStore.Core.Models.ServiceModels.Publishers;

namespace GameStore.Core.Profiles;

public class CoreCommonProfile : Profile
{
    public CoreCommonProfile()
    {
        CreateMap<GameCreateModel, Game>();

        CreateMap<GenreCreateModel, Genre>();

        CreateMap<PlatformTypeCreateModel, PlatformType>();

        CreateMap<PublisherCreateModel, Publisher>();

        CreateMap<GameCreateModel, Game>()
            .ForMember(game => game.Genres,
                       expression => expression.MapFrom(createModel => createModel.GenresIds.Select(
                                                            id => new GameGenre { GenreId = id })))
            .ForMember(game => game.Platforms,
                       expression => expression.MapFrom(createModel =>
                                                            createModel.PlatformsIds.Select(
                                                                id => new GamePlatformType { PlatformId = id })));

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