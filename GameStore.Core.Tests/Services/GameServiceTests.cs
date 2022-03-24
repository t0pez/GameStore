using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.RelationshipModelsServices;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.Core.Models.Records;
using GameStore.Core.Models.RelationalModels;
using GameStore.Core.Profiles;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services;

public class GameServiceTests
{
    private readonly IGameService _gameService;
    private readonly Mock<IUnitOfWork> _mockUoW;
    
    public GameServiceTests()
    {
        _mockUoW = GetUnitOfWorkMock();
        
        IMapper mapper = new Mapper(new MapperConfiguration(expression => expression.AddProfile(new CoreCommonProfile())));
        var mockLogger = new Mock<ILogger<GameService>>();
        var mockGameGenreService = new Mock<IRelationshipModelService<GameGenre>>();
        var mockGamePlatformService = new Mock<IRelationshipModelService<GamePlatformType>>();
        
        _gameService = new GameService(_mockUoW.Object, mockLogger.Object, mapper,
                                       mockGameGenreService.Object, mockGamePlatformService.Object);
    }

    [Fact]
    public async void GetAllAsync_ReturnsCorrectValues()
    {
        const int expectedCount = 4;

        var actualResult = await _gameService.GetAllAsync();
        var actualCount = actualResult.Count;

        Assert.Equal(expectedCount, actualCount);
    }
    
    [Fact]
    public async void GetByKeyAsync_ExistingKey_ReturnsGames()
    {
        var expectedResult = AllGames[0];

        var actualResult = await _gameService.GetByKeyAsync("first-game");
        
        Assert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async void GetByKeyAsync_NotExistingKey_ThrowsNotFoundException()
    {
        var operation = async () => await _gameService.GetByKeyAsync("first-gameeee");

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }

    [Fact]
    public async void CreateAsync_CorrectValues_ReturnsGame()
    {
        var expectedResult = AllGames[0];
        expectedResult.Id = Guid.Empty;
        var createModel = new GameCreateModel
        {
            Name = expectedResult.Name,
            Description = expectedResult.Description,
            File = expectedResult.File
        };

        var actualResult = await _gameService.CreateAsync(createModel);
        
        Assert.Equal(expectedResult.Id, actualResult.Id);
    }

    [Fact]
    public async void DeleteAsync_CorrectValues()
    {
        await _gameService.DeleteAsync(AllGames[3].Id);
        
        Assert.True(true);
        _mockUoW.Verify(work => work.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void DeleteAsync_NotCorrectValues_ThrowsNotFoundException()
    {
        var operation = async () => await _gameService.DeleteAsync(Guid.Empty);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }

    [Fact]
    public async void GetFileAsync_ExistingKey_ReturnsFile()
    {
        var expectedResult = AllGames[0].File;

        var actualResult = await _gameService.GetFileAsync(AllGames[0].Key);
        
        Assert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async void GetFileAsync_NotExistingKey_ThrowsNotFoundException()
    {
        var operation = async () => await _gameService.GetFileAsync("FirstGame.Key");

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }

    private static Mock<IUnitOfWork> GetUnitOfWorkMock()
    {
        var mock = new Mock<IUnitOfWork>();
        var gameRepository = GetGameRepositoryMock();
        
        mock.Setup(unitOfWork => unitOfWork.GetRepository<Game>())
            .Returns(gameRepository.Object);
        mock.Setup(unitOfWork => unitOfWork.SaveChangesAsync())
            .ReturnsAsync(0)
            .Verifiable();

        return mock;
    }

    private static Mock<IRepository<Game>> GetGameRepositoryMock()
    {
        var mock = new Mock<IRepository<Game>>();
        
        mock.Setup(repository => repository.GetBySpecAsync(It.IsAny<GamesWithDetailsSpec>()))
            .ReturnsAsync(AllGames);
        
        mock.Setup(repository => repository.GetBySpecAsync(null))
            .ReturnsAsync(AllGames);

        mock.Setup(repository => repository.GetSingleBySpecAsync(
                       It.Is<GameByKeyWithDetailsSpec>(spec => spec.Key == AllGames[0].Key)))
            .ReturnsAsync(AllGames[0]);
        
        mock.Setup(repository => repository.GetSingleBySpecAsync(
                       It.Is<GameByIdSpec>(spec => spec.Id == AllGames[3].Id)))
            .ReturnsAsync(AllGames[3]);

        mock.Setup(repository => repository.AddAsync(It.IsAny<Game>()))
            .Returns(Task.CompletedTask);

        mock.Setup(repository => repository.UpdateAsync(It.IsAny<Game>()))
            .Returns(Task.CompletedTask);

        mock.Setup(repository => repository.DeleteAsync(It.IsAny<Game>()))
            .Returns(Task.CompletedTask);

        return mock;
    }

    private static readonly List<Game> AllGames = new()
    {
        new()
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601"),
            Name = "First game",
            Key = "first-game",
            Description = "First description",
            File = new byte[] { 0, 0, 0, 1 },
            IsDeleted = false
        },
        new()
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601"),
            Name = "Second game",
            Key = "second-game",
            Description = "Second description",
            File = new byte[] { 0, 0, 0, 2 },
            IsDeleted = false
        },
        new()
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b973-08da067d7601"),
            Name = "Third game",
            Key = "Third-game",
            Description = "Third description",
            File = new byte[] { 0, 0, 0, 3 },
            IsDeleted = false
        },
        new()
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601"),
            Name = "Fourth game",
            Key = "fourth-game",
            Description = "Fourth description",
            File = new byte[] { 0, 0, 0, 4 },
            IsDeleted = false
        }
    };
}