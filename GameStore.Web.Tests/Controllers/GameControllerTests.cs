using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Records;
using GameStore.Web.Controllers;
using GameStore.Web.Models;
using GameStore.Web.Profiles;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers;

public class GameControllerTests
{
    private readonly GamesController _gameController;
    private readonly Mock<IGameService> _gameServiceMock;
    private readonly Mock<ICommentService> _commentServiceMock;

    public GameControllerTests()
    {
        _gameServiceMock = GetGameServiceMock();
        _commentServiceMock = GetCommentServiceMock();

        _gameController = new GamesController(
            _gameServiceMock.Object, _commentServiceMock.Object,
            new Mapper(new MapperConfiguration(expression => expression.AddProfile(new WebCommonProfile()))));
    }

    [Fact]
    public async void GetAllAsync_NoParameters_ReturnsCorrectValue()
    {
        var expectedResultCount = AllGamesViewModels.Count;

        var actualResult = await _gameController.GetAllAsync();
        var actualResultCount = actualResult.Count;
        
        Assert.Equal(expectedResultCount, actualResultCount);
    }
    
    [Fact]
    public async void GetWithDetailsAsync_ExistingKey_ReturnsGame()
    {
        var expectedModel = AllGamesViewModels[0];

        var actualResult = await _gameController.GetWithDetailsAsync(AllGames[0].Key);
        var actualObjectResult = actualResult.Result as OkObjectResult;
        var actualResultModel = actualObjectResult?.Value as GameViewModel;

        Assert.IsType<OkObjectResult>(actualResult.Result);
        Assert.IsType<GameViewModel>(actualResultModel);
        Assert.Equal(expectedModel.Id, actualResultModel.Id);
        Assert.Equal(expectedModel.Name, actualResultModel.Name);
    }
    
    [Fact]
    public async void GetWithDetailsAsync_NotExistingKey_ThrowsNotFoundException()
    {
        var operation = async () => await _gameController.GetWithDetailsAsync("AllGames[0].Key");

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }
    
    [Fact]
    public async void GetFileAsync_ExistingKey_ReturnsGame()
    {
        var expectedResult = AllGames[0].File;

        var actualResult = await _gameController.GetFileAsync(AllGames[0].Key);
        
        Assert.Equal(expectedResult, (actualResult.Result as OkObjectResult)?.Value);
    }
    
    [Fact]
    public async void GetFileAsync_NotExistingKey_ThrowsNotFoundException()
    {
        var operation = async () => await _gameController.GetWithDetailsAsync("AllGames[0].Key");

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }

    [Fact]
    public async void CreateAsync_CorrectValue_ReturnsModel()
    {
        var expectedGameKey = AllGames[1].Key;
        var createModel = new GameCreateRequestModel
        {
            Name = AllGames[1].Name,
            Description = AllGames[1].Description,
            File = AllGames[1].File
        };

        var actualResult = await _gameController.CreateAsync(createModel);
        var actualObjectResult = actualResult.Result as OkObjectResult;
        var actualResultModel = actualObjectResult?.Value as GameViewModel;

        Assert.IsType<OkObjectResult>(actualResult.Result);
        Assert.IsType<GameViewModel>(actualResultModel);
        Assert.Equal(expectedGameKey, actualResultModel.Key);
    }
    
    [Fact]
    public async void CommentGameAsync_CorrectValue()
    {
        var createModel = new CommentCreateRequestModel
        {
            GameKey = AllGames[2].Key,
            AuthorName = "Some author",
            Message = "Some text"
        };

        var actualResult = await _gameController.CommentGameAsync(createModel);

        Assert.IsType<OkResult>(actualResult);
    }

    [Fact]
    public async void UpdateAsync_CorrectValues()
    {
        var editModel = new GameEditRequestModel
        {
            Id = AllGames[1].Id,
            Name = AllGames[1].Name,
            Description = AllGames[1].Description,
            File = AllGames[1].File,
            GenresIds = AllGames[1].Genres.Select(gg => gg.GenreId).ToList(),
            PlatformsIds = AllGames[1].Platforms.Select(gp => gp.PlatformId).ToList()
        };

        var actualResult = await _gameController.UpdateAsync(editModel);

        Assert.IsType<OkResult>(actualResult);
    }
    
    [Fact]
    public async void DeleteAsync_CorrectValues()
    {
        var idToDelete = AllGames[3].Id;
        
        var actualResult = await _gameController.DeleteAsync(idToDelete);

        Assert.IsType<OkResult>(actualResult);
    }
    
    private Mock<IGameService> GetGameServiceMock()
    {
        var mock = new Mock<IGameService>();

        mock.Setup(service => service.GetAllAsync())
            .ReturnsAsync(AllGames);

        mock.Setup(service => service.GetByKeyAsync(AllGames[0].Key))
            .ReturnsAsync(AllGames[0]);

        mock.Setup(service => service.GetFileAsync(AllGames[0].Key))
            .ReturnsAsync(AllGames[0].File);

        mock.Setup(service => service.GetByKeyAsync("AllGames[0].Key"))
            .ThrowsAsync(new ItemNotFoundException());

        mock.Setup(service => service.GetFileAsync("AllGames[0].Key"))
            .ThrowsAsync(new ItemNotFoundException());

        mock.Setup(service => service.CreateAsync(
                       It.Is<GameCreateModel>(model => model.Name == AllGames[1].Name)))
            .ReturnsAsync(AllGames[1]);

        return mock;
    }
    
    private Mock<ICommentService> GetCommentServiceMock()
    {
        var mock = new Mock<ICommentService>();

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
    
    private static readonly List<GameViewModel> AllGamesViewModels = new()
    {
        new()
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601"),
            Name = "First game",
            Key = "first-game",
            Description = "First description",
            Genres = new List<GenreViewModel>(),
            Platforms = new List<PlatformTypeViewModel>()
        },
        new()
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601"),
            Name = "Second game",
            Key = "second-game",
            Description = "Second description",
            Genres = new List<GenreViewModel>(),
            Platforms = new List<PlatformTypeViewModel>()
        },
        new()
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b973-08da067d7601"),
            Name = "Third game",
            Key = "Third-game",
            Description = "Third description",
            Genres = new List<GenreViewModel>(),
            Platforms = new List<PlatformTypeViewModel>()
        },
        new()
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601"),
            Name = "Fourth game",
            Key = "fourth-game",
            Description = "Fourth description",
            Genres = new List<GenreViewModel>(),
            Platforms = new List<PlatformTypeViewModel>()
        }
    };
}