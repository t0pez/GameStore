using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.Loggers;
using GameStore.Core.Interfaces.RelationshipModelsServices;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.Core.Models.Mongo.Products;
using GameStore.Core.Models.RelationalModels;
using GameStore.Core.Models.ServiceModels.Enums;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services;

public class GameServiceTests
{
    private readonly IGameService _gameService;
    private readonly Mock<ISearchService> _searchServiceMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Game>> _gameRepoMock;
    private readonly Mock<IRepository<Product>> _productRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRelationshipModelService<GameGenre>> _gameGenreServiceMock;
    private readonly Mock<IRelationshipModelService<GamePlatformType>> _gamePlatformServiceMock;

    public GameServiceTests()
    {
        var loggerMock = new Mock<ILogger<GameService>>();
        var mongoLoggerMock = new Mock<IMongoLogger>();
        _mediatorMock = new Mock<IMediator>();
        _searchServiceMock = new Mock<ISearchService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _gameRepoMock = new Mock<IRepository<Game>>();
        _productRepoMock = new Mock<IRepository<Product>>();
        _mapperMock = new Mock<IMapper>();
        _gameGenreServiceMock = new Mock<IRelationshipModelService<GameGenre>>();
        _gamePlatformServiceMock = new Mock<IRelationshipModelService<GamePlatformType>>();

        _unitOfWorkMock.Setup(unit => unit.GetEfRepository<Game>())
                       .Returns(_gameRepoMock.Object);
        _unitOfWorkMock.Setup(unit => unit.GetMongoRepository<Product>())
                       .Returns(_productRepoMock.Object);

        _gameService = new GameService(_searchServiceMock.Object, _gameGenreServiceMock.Object,
                                       _gamePlatformServiceMock.Object, _mediatorMock.Object, mongoLoggerMock.Object,
                                       _unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async void GetByIdAsync_ExistingId_ReturnsGames()
    {
        var gameId = Guid.NewGuid();
        var game = new Game { Id = gameId };

        _gameRepoMock.Setup(repository =>
                                repository.GetSingleOrDefaultBySpecAsync(
                                    It.Is<GameByIdSpec>(spec => spec.Id == gameId)))
                     .ReturnsAsync(game);

        var actualResult = await _gameService.GetByIdAsync(gameId);

        actualResult.Id.Should().Be(gameId);
    }

    [Fact]
    public async void GetByIdAsync_NotExistingId_ThrowsNotFoundException()
    {
        var gameId = Guid.NewGuid();
        Game game = null!;

        _gameRepoMock
            .Setup(repository =>
                       repository.GetSingleOrDefaultBySpecAsync(
                           It.Is<GameByIdSpec>(spec => spec.Id == gameId)))
            .ReturnsAsync(game);

        var operation = async () => await _gameService.GetByIdAsync(gameId);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }
    
    [Fact]
    public async void GetByKeyAsync_ExistingKey_ReturnsGames()
    {
        const string gameKey = "existing-game-key";
        var gameId = Guid.NewGuid();

        _gameRepoMock.Setup(repository =>
                                repository.GetSingleOrDefaultBySpecAsync(
                                    It.Is<GameByKeyWithDetailsSpec>(spec => spec.Key == gameKey)))
                     .ReturnsAsync(new Game { Id = gameId, Key = gameKey });

        var actualResult = await _gameService.GetByKeyAsync(gameKey);
        var actualId = actualResult.Id;
        var actualGameKey = actualResult.Key;

        Assert.Equal(gameId, actualId);
        Assert.Equal(gameKey, actualGameKey);
    }

    [Fact]
    public async void GetByKeyAsync_NotExistingKey_ThrowsNotFoundException()
    {
        const string gameKey = "not-existing-game-key";
        Game gameByKey = null!;

        _gameRepoMock
            .Setup(repository =>
                       repository.GetSingleOrDefaultBySpecAsync(
                           It.Is<GameByKeyWithDetailsSpec>(spec => spec.Key == gameKey)))
            .ReturnsAsync(gameByKey);

        var operation = async () => await _gameService.GetByKeyAsync(gameKey);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }

    [Fact]
    public async void CreateAsync_CorrectValues_ReturnsGame()
    {
        var expectedId = Guid.NewGuid();
        var createModel = new GameCreateModel
        {
            Name = "Some name",
            Key = "some-name"
        };

        _gameRepoMock
            .Setup(repository => repository.AnyAsync(It.Is<GameByKeySpec>(spec => spec.Key == createModel.Key)))
            .ReturnsAsync(false);
        _mapperMock.Setup(mapper => mapper.Map<Game>(It.IsAny<GameCreateModel>()))
                   .Returns(new Game { Id = expectedId });

        var actualResult = await _gameService.CreateAsync(createModel);

        Assert.Equal(expectedId, actualResult.Id);
        _gameRepoMock.Verify(repository => repository.AddAsync(It.IsAny<Game>()), Times.Once);
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void GetTotalCountAsync_NoParameters_ReturnsCorrectValues()
    {
        const int expectedCount = 4;
        var expectedServerItems = new List<Game>
        {
            new()
            {
                Key = "key1"
            },
            new()
            {
                Key = "key2"
            }
        };
        var expectedMongoItems = new List<Product>
        {
            new()
            {
                GameKey = "key3"
            },
            new()
            {
                GameKey = "key4"
            }
        };
        var mappedServerItems = new List<ProductDto>
        {
            new()
            {
                Key = "key1"
            },
            new()
            {
                Key = "key2"
            }
        };
        var mappedMongoItems = new List<ProductDto>
        {
            new()
            {
                Key = "key3"
            },
            new()
            {
                Key = "key4"
            }
        };

        _gameRepoMock.Setup(repository => repository.GetBySpecAsync(It.IsAny<GamesListSpec>()))
                     .ReturnsAsync(expectedServerItems);
        _productRepoMock.Setup(repository => repository.GetBySpecAsync(null))
                     .ReturnsAsync(expectedMongoItems);
        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<ProductDto>>(expectedServerItems))
                   .Returns(mappedServerItems);
        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<ProductDto>>(expectedMongoItems))
                   .Returns(mappedMongoItems);

        var actualResult = await _gameService.GetTotalCountAsync();

        actualResult.Should().Be(expectedCount);
    }

    [Fact]
    public async void DeleteAsync_ServerEntity_GameSoftDeleted()
    {
        const string gameKey = "game-key";
        var gameId = Guid.NewGuid();
        var dto = new ProductDto
        {
            Key = gameKey,
            Database = Database.Server
        };
        var game = new Game
        {
            Id = gameId,
            Key = gameKey
        };
        var gameUpdateModel = new GameUpdateModel
        {
            OldGameKey = gameKey,
            Key = gameKey,
            IsDeleted = true
        };
        
        _searchServiceMock.Setup(service => service.GetProductDtoByGameKeyOrDefaultAsync(gameKey))
                          .ReturnsAsync(dto);
        _mapperMock.Setup(mapper => mapper.Map<GameUpdateModel>(
                              It.Is<ProductDto>(productDto =>
                                                    productDto.Key == gameKey && productDto.IsDeleted == true)))
                   .Returns(gameUpdateModel);
        _gameRepoMock.Setup(repository =>
                                repository.GetSingleOrDefaultBySpecAsync(
                                    It.Is<GameByKeyWithDetailsSpec>(spec => spec.Key == gameKey)))
                     .ReturnsAsync(game);

        await _gameService.DeleteAsync(gameKey, Database.Server);

        _gameRepoMock.Verify(
            repository =>
                repository.UpdateAsync(It.Is<Game>(game => game.Key == gameKey && game.IsDeleted == true)),
            Times.Once);
        _unitOfWorkMock.Verify(work => work.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void DeleteAsync_NotCorrectValues_ThrowsNotFoundException()
    {
        const string notExistingKey = "not-existing-key";
        ProductDto gameByKey = null!;

        _searchServiceMock.Setup(service =>
                                service.GetProductDtoByGameKeyOrDefaultAsync(notExistingKey))
                     .ReturnsAsync(gameByKey);

        var operation = async () => await _gameService.DeleteAsync(notExistingKey, Database.Server);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }

    [Fact]
    public async void GetFileAsync_ExistingKey_ReturnsFile()
    {
        const string gameKey = "existing-game-key";
        var expectedResult = new byte[] { 0, 0, 0, 0 };

        _gameRepoMock.Setup(repository =>
                                repository.GetSingleOrDefaultBySpecAsync(
                                    It.Is<GameByKeySpec>(spec => spec.Key == gameKey)))
                     .ReturnsAsync(new Game { File = expectedResult });

        var actualResult = await _gameService.GetFileAsync(gameKey);

        Assert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async void GetFileAsync_NotExistingKey_ThrowsNotFoundException()
    {
        const string gameKey = "existing-game-key";
        Game gameByKey = null!;

        _gameRepoMock.Setup(repository =>
                                repository.GetSingleOrDefaultBySpecAsync(
                                    It.Is<GameByKeySpec>(spec => spec.Key == gameKey)))
                     .ReturnsAsync(gameByKey);

        var operation = async () => await _gameService.GetFileAsync(gameKey);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }
}