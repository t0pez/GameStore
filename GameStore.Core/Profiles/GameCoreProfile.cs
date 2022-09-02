using System;
using System.Linq;
using AutoMapper;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Mongo.Products;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.RelationalModels;
using GameStore.Core.Models.ServiceModels.Games;

namespace GameStore.Core.Profiles;

public class GameCoreProfile : Profile
{
    private static readonly DateTime AddedToStoreAtForMongo = new(2022, 6, 2);

    public GameCoreProfile()
    {
        CreateMap<ProductDto, GameCreateModel>()
            .ForMember(model => model.GenresIds,
                       expression => expression.MapFrom(dto => dto.Genres.Select(genre => genre.Id)))
            .ForMember(model => model.PlatformsIds,
                       expression => expression.MapFrom(dto => dto.Platforms.Select(platform => platform.Id)));

        CreateMap<ProductDto, GameUpdateModel>()
            .ForMember(model => model.GenresIds,
                       expression => expression.MapFrom(dto => dto.Genres.Select(genre => genre.Id)))
            .ForMember(model => model.PlatformsIds,
                       expression => expression.MapFrom(dto => dto.Platforms.Select(platform => platform.Id)));

        CreateMap<ProductDto, Game>()
            .ForMember(game => game.Id,
                       expression => expression.MapFrom(dto => Guid.Parse(dto.Id)))
            .ForMember(game => game.Genres,
                       expression =>
                           expression.MapFrom(dto => dto.Genres.Select(
                                                  genre => new GameGenre
                                                      { GameId = Guid.Parse(dto.Id), GenreId = genre.Id })))
            .ForMember(game => game.Platforms,
                       expression =>
                           expression.MapFrom(dto => dto.Platforms.Select(
                                                  platform => new GamePlatformType
                                                      { GameId = Guid.Parse(dto.Id), PlatformId = platform.Id })));

        CreateMap<GameCreateModel, Game>()
            .ForMember(game => game.Description,
                       expression => expression.MapFrom(model => model.Description ?? string.Empty))
            .ForMember(game => game.PublisherName,
                       expression => expression.MapFrom(model => model.PublisherName ?? string.Empty))
            .ForMember(game => game.Genres,
                       expression => expression.MapFrom(createModel => createModel.GenresIds.Select(
                                                            id => new GameGenre { GenreId = id })))
            .ForMember(game => game.Platforms,
                       expression => expression.MapFrom(createModel =>
                                                            createModel.PlatformsIds.Select(
                                                                id => new GamePlatformType { PlatformId = id })));

        CreateMap<Game, GameUpdateModel>()
            .ForMember(updateModel => updateModel.GenresIds,
                       expression => expression.MapFrom(game => game.Genres.Select(gg => gg.GenreId)))
            .ForMember(updateModel => updateModel.PlatformsIds,
                       expression => expression.MapFrom(game => game.Platforms.Select(gp => gp.PlatformId)));

        CreateMap<Game, ProductDto>()
            .ForMember(productDto => productDto.Id,
                       expression => expression.MapFrom(game => game.Id.ToString()))
            .ForMember(productDto => productDto.Genres,
                       expression => expression.MapFrom(game => game.Genres.Select(gg => gg.Genre)))
            .ForMember(productDto => productDto.Platforms,
                       expression => expression.MapFrom(game => game.Platforms.Select(gp => gp.Platform)));

        CreateMap<Product, ProductDto>()
            .ForMember(dto => dto.Id, expression => expression.MapFrom(product => product.Id.ToString()))
            .ForMember(productDto => productDto.Name,
                       expression => expression.MapFrom(product => product.ProductName))
            .ForMember(productDto => productDto.Key,
                       expression => expression.MapFrom(product => product.GameKey))
            .ForMember(productDto => productDto.Price,
                       expression => expression.MapFrom(product => product.UnitPrice))
            .ForMember(productDto => productDto.Publisher,
                       expression => expression.MapFrom(product => product.Supplier))
            .ForMember(dto => dto.AddedToStoreAt,
                       expression => expression.MapFrom(product => AddedToStoreAtForMongo));

        CreateMap<Product, GameUpdateModel>();

        CreateMap<GameUpdateModel, Product>()
            .ForMember(product => product.ProductName,
                       expression => expression.MapFrom(model => model.Name))
            .ForMember(product => product.GameKey,
                       expression => expression.MapFrom(model => model.Key))
            .ForMember(product => product.UnitPrice,
                       expression => expression.MapFrom(model => model.Price))
            .ForMember(product => product.Views,
                       expression => expression.MapFrom(model => model.Views))
            .ForMember(product => product.ProductName, expression => expression.Ignore())
            .ForMember(product => product.QuantityPerUnit, expression => expression.Ignore())
            .ForMember(product => product.Discontinued, expression => expression.Ignore())
            .ForMember(product => product.CategoryId, expression => expression.Ignore())
            .ForMember(product => product.Category, expression => expression.Ignore())
            .ForMember(product => product.SupplierId, expression => expression.Ignore())
            .ForMember(product => product.Supplier, expression => expression.Ignore())
           .ForMember(product => product.Database, expression => expression.Ignore());
    }
}