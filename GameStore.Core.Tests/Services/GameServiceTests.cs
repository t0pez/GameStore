using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.RelationshipModelsServices;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.Core.Models.RelationalModels;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services;

public class GameServiceTests
{
    private readonly IGameService _gameService;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Game>> _gameRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRelationshipModelService<GameGenre>> _gameGenreServiceMock;
    private readonly Mock<IRelationshipModelService<GamePlatformType>> _gamePlatformServiceMock;

    public GameServiceTests()
    {
        var loggerMock = new Mock<ILogger<GameService>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _gameRepoMock = new Mock<IRepository<Game>>();
        _mapperMock = new Mock<IMapper>();
        _gameGenreServiceMock = new Mock<IRelationshipModelService<GameGenre>>();
        _gamePlatformServiceMock = new Mock<IRelationshipModelService<GamePlatformType>>();

        _unitOfWorkMock.Setup(unit => unit.GetRepository<Game>())
                       .Returns(_gameRepoMock.Object);

        _gameService = new GameService(_unitOfWorkMock.Object, loggerMock.Object, _mapperMock.Object,
                                       _gameGenreServiceMock.Object, _gamePlatformServiceMock.Object);
    }

    [Fact]
    public async void GetAllAsync_ReturnsCorrectValues()
    {
        const int expectedCount = 4;

        _gameRepoMock.Setup(repository => repository.GetBySpecAsync(It.IsAny<GamesListSpec>()))
                     .ReturnsAsync(new List<Game>(new Game[expectedCount]));

        var actualResult = await _gameService.GetAllAsync();
        var actualCount = actualResult.Count;

        Assert.Equal(expectedCount, actualCount);
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

        _gameRepoMock.Setup(repository => repository.CountAsync(It.IsAny<GamesListSpec>()))
                     .ReturnsAsync(expectedCount);

        var actualResult = await _gameService.GetTotalCountAsync();

        actualResult.Should().Be(expectedCount);
    }

    [Fact]
    public async void DeleteAsync_CorrectValues_GameSoftDeleted()
    {
        var expectedId = Guid.NewGuid();
        _gameRepoMock.Setup(repository =>
                                repository.GetSingleOrDefaultBySpecAsync(
                                    It.Is<GameByIdSpec>(spec => spec.Id == expectedId)))
                     .ReturnsAsync(new Game());

        await _gameService.DeleteAsync(expectedId);

        _gameRepoMock.Verify(repository => repository.UpdateAsync(It.IsAny<Game>()), Times.Once);
        _unitOfWorkMock.Verify(work => work.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void DeleteAsync_NotCorrectValues_ThrowsNotFoundException()
    {
        Game gameByKey = null!;

        _gameRepoMock.Setup(repository =>
                                repository.GetSingleOrDefaultBySpecAsync(
                                    It.Is<GameByIdSpec>(spec => spec.Id == Guid.Empty)))
                     .ReturnsAsync(gameByKey);

        var operation = async () => await _gameService.DeleteAsync(Guid.Empty);

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