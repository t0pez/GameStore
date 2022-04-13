﻿using AutoMapper;
using GameStore.Core.Models.Baskets;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Publishers;
using GameStore.Core.Models.Records;
using GameStore.Core.Models.RelationalModels;
using GameStore.Web.Models.Basket;
using GameStore.Web.Models.Comment;
using GameStore.Web.Models.Game;
using GameStore.Web.Models.Publisher;
using GameStore.Web.ViewModels.Basket;
using GameStore.Web.ViewModels.Comments;
using GameStore.Web.ViewModels.Games;
using GameStore.Web.ViewModels.Publisher;

namespace GameStore.Web.Profiles;

public class WebCommonProfile : Profile
{
    public WebCommonProfile()
    {
        CreateMap<GameCreateRequestModel, GameCreateModel>().ReverseMap();
        CreateMap<GameEditRequestModel, GameUpdateModel>().ReverseMap();

        CreateMap<CommentCreateRequestModel, CommentCreateModel>().ReverseMap();
        CreateMap<ReplyCreateRequestModel, ReplyCreateModel>().ReverseMap();

        CreateMap<PublisherCreateRequestModel, PublisherCreateModel>().ReverseMap();

        CreateMap<Basket, BasketViewModel>();
        CreateMap<Basket, BasketCookieModel>().ReverseMap();

        CreateMap<Game, GameViewModel>();
        CreateMap<Game, GameListViewModel>();
        CreateMap<Game, GameInBasketViewModel>();

        CreateMap<Genre, GenreViewModel>();
        CreateMap<Publisher, PublisherViewModel>();

        CreateMap<BasketItem, BasketItemViewModel>();
        CreateMap<BasketItem, BasketItemCookieModel>()
            .ForMember(model => model.GameId,
                       expression => expression.MapFrom(basketItem => basketItem.Game.Id));
        CreateMap<BasketItemCookieModel, BasketItem>()
            .ForMember(item => item.Game,
                       expression => expression.MapFrom(model => new Game { Id = model.GameId }));

        CreateMap<Comment, CommentViewModel>()
            .ForMember(model => model.GameKey,
                       expression => expression.MapFrom(comment => comment.Game.Key))
            .ForMember(model => model.AuthorName,
                       expression => expression.MapFrom(comment => comment.Name));

        CreateMap<GameGenre, GenreViewModel>()
            .ForMember(viewModel => viewModel.Id,
                       expression => expression.MapFrom(gameGenre => gameGenre.GenreId))
            .ForMember(viewModel => viewModel.Name,
                       expression => expression.MapFrom(gameGenre => gameGenre.Genre.Name))
            .ForMember(viewModel => viewModel.SubGenres,
                       expression => expression.MapFrom(gameGenre => gameGenre.Genre.SubGenres));

        CreateMap<GamePlatformType, PlatformTypeViewModel>()
            .ForMember(viewModel => viewModel.Id,
                       expression => expression.MapFrom(gameGenre => gameGenre.PlatformId))
            .ForMember(viewModel => viewModel.Name,
                       expression => expression.MapFrom(gameGenre => gameGenre.Platform.Name));
    }
}