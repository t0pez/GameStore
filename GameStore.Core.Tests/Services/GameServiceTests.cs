using System;
using System.Collections.Generic;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.RelationshipModelsServices;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.Core.Models.Records;
using GameStore.Core.Models.RelationalModels;
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
    private readonly Mock<ILogger<GameService>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRelationshipModelService<GameGenre>> _gameGenreServiceMock;
    private readonly Mock<IRelationshipModelService<GamePlatformType>> _gamePlatformServiceMock;
    
    
    public GameServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _gameRepoMock = new Mock<IRepository<Game>>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GameService>>();
        _gameGenreServiceMock = new Mock<IRelationshipModelService<GameGenre>>();
        _gamePlatformServiceMock = new Mock<IRelationshipModelService<GamePlatformType>>();

        _unitOfWorkMock.Setup(unit => unit.GetRepository<Game>())
                .Returns(_gameRepoMock.Object);
        
        _gameService = new GameService(_unitOfWorkMock.Object, _loggerMock.Object, _mapperMock.Object,
                                       _gameGenreServiceMock.Object, _gamePlatformServiceMock.Object);
    }

    [Fact]
    public async void GetAllAsync_ReturnsCorrectValues()
    {
        const int expectedCount = 4;

        _gameRepoMock.Setup(repository => repository.GetBySpecAsync(It.IsAny<GamesWithDetailsSpec>()))
                     .ReturnsAsync(new List<Game>(new Game[expectedCount]));

        var actualResult = await _gameService.GetAllAsync();
        var actualCount = actualResult.Count;

        Assert.Equal(expectedCount, actualCount);
    }
    
    [Fact]
    public async void GetByKeyAsync_ExistingKey_ReturnsGames()
    {
        const string gameKey = "existing-game-key";
        var gameId = Guid.NewGuid();

        _gameRepoMock.Setup(repository => repository.GetSingleBySpecAsync(It.IsAny<GameByKeyWithDetailsSpec>()))
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
        
        _gameRepoMock.Setup(repository => repository.GetSingleBySpecAsync(It.IsAny<GameByKeyWithDetailsSpec>()))
                     .ReturnsAsync((Game)null!);
        
        var operation = async () => await _gameService.GetByKeyAsync(gameKey);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }

    [Fact]
    public async void CreateAsync_CorrectValues_ReturnsGame()
    {
        var expectedId = Guid.NewGuid();
        var createModel = new GameCreateModel
        {
            Name = "Some name"
        };
        
        _gameRepoMock.Setup(repository => repository.AnyAsync(It.IsAny<GameByKeySpec>()))
                     .ReturnsAsync(false);
        _mapperMock.Setup(mapper => mapper.Map<Game>(It.IsAny<GameCreateModel>()))
                   .Returns(new Game {Id = expectedId });

        var actualResult = await _gameService.CreateAsync(createModel);
        
        Assert.Equal(expectedId, actualResult.Id);
        _gameRepoMock.Verify(repository => repository.AddAsync(It.IsAny<Game>()), Times.Once);
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async void CreateAsync_WithExistingKey_GeneratesNewKey()
    {
        var expectedId = Guid.NewGuid();
        const string existingKey = "some-key";
        const string expectedKey = "some-key--1";
        var createModel = new GameCreateModel
        {
            Name = "Some name"
        };
        
        _gameRepoMock.Setup(repository => repository.AnyAsync(It.Is<GameByKeySpec>(spec => spec.Key == existingKey)))
                     .ReturnsAsync(true);
        _gameRepoMock.Setup(repository => repository.AnyAsync(It.Is<GameByKeySpec>(spec => spec.Key == expectedKey)))
                     .ReturnsAsync(false);
        _mapperMock.Setup(mapper => mapper.Map<Game>(It.IsAny<GameCreateModel>()))
                   .Returns(new Game {Id = expectedId });

        var actualResult = await _gameService.CreateAsync(createModel);
        
        Assert.Equal(expectedId, actualResult.Id);
        _gameRepoMock.Verify(repository => repository.AddAsync(It.IsAny<Game>()), Times.Once);
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void DeleteAsync_CorrectValues_GameSoftDeleted()
    {
        _gameRepoMock.Setup(repository => repository.GetSingleBySpecAsync(It.IsAny<GameByIdSpec>()))
                     .ReturnsAsync(new Game());
        
        await _gameService.DeleteAsync(Guid.NewGuid());
        
        _gameRepoMock.Verify(repository => repository.UpdateAsync(It.IsAny<Game>()), Times.Once);
        _unitOfWorkMock.Verify(work => work.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void DeleteAsync_NotCorrectValues_ThrowsNotFoundException()
    {
        _gameRepoMock.Setup(repository => repository.GetSingleBySpecAsync(It.IsAny<GameByIdSpec>()))
                     .ReturnsAsync((Game)null!);
        
        var operation = async () => await _gameService.DeleteAsync(Guid.Empty);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }

    [Fact]
    public async void GetFileAsync_ExistingKey_ReturnsFile()
    {
        const string gameKey = "existing-game-key";
        var expectedResult = new byte[] { 0, 0, 0, 0 };

        _gameRepoMock.Setup(repository => repository.GetSingleBySpecAsync(It.IsAny<GameByKeySpec>()))
                     .ReturnsAsync(new Game { File = expectedResult });

        var actualResult = await _gameService.GetFileAsync(gameKey);
        
        Assert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async void GetFileAsync_NotExistingKey_ThrowsNotFoundException()
    {
        const string gameKey = "existing-game-key";

        _gameRepoMock.Setup(repository => repository.GetSingleBySpecAsync(It.IsAny<GameByKeySpec>()))
                     .ReturnsAsync((Game)null!);
        
        var operation = async () => await _gameService.GetFileAsync(gameKey);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }
}