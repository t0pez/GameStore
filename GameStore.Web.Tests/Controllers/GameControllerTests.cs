using System;
using System.Collections.Generic;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Records;
using GameStore.Web.Controllers;
using GameStore.Web.Models;
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
    private readonly Mock<IMapper> _mapperMock;

    public GameControllerTests()
    {
        _gameServiceMock = new Mock<IGameService>();
        _commentServiceMock = new Mock<ICommentService>();
        _mapperMock = new Mock<IMapper>();

        _gameController = new GamesController(_gameServiceMock.Object, _commentServiceMock.Object,
                                              _mapperMock.Object);
    }

    [Fact]
    public async void GetAllAsync_NoParameters_ReturnsCorrectValue()
    {
        const int expectedResultCount = 4;

        _gameServiceMock.Setup(service => service.GetAllAsync())
                        .ReturnsAsync(new List<Game>(new Game[expectedResultCount]));
        _mapperMock.Setup(service => service.Map<ICollection<GameViewModel>>(It.IsAny<ICollection<Game>>()))
                        .Returns(new List<GameViewModel>(new GameViewModel[expectedResultCount]));
        
        var actualResult = await _gameController.GetAllAsync();
        var actualResultCount = actualResult.Count;
        
        Assert.Equal(expectedResultCount, actualResultCount);
    }
    
    [Fact]
    public async void GetWithDetailsAsync_ExistingKey_ReturnsGame()
    {
        var expectedId = Guid.NewGuid();
        const string gameKey = "existing-game-key";

        _gameServiceMock.Setup(service => service.GetByKeyAsync(gameKey))
                        .ReturnsAsync(new Game {Id = expectedId});
        _mapperMock.Setup(mapper => mapper.Map<GameViewModel>(It.IsAny<Game>()))
                   .Returns(new GameViewModel { Id = expectedId });

        var actualResult = await _gameController.GetWithDetailsAsync(gameKey);

        var actualObjectResult = Assert.IsType<OkObjectResult>(actualResult.Result);
        var actualResultModel = Assert.IsType<GameViewModel>(actualObjectResult.Value);
        Assert.Equal(expectedId, actualResultModel.Id);
    }
    
    [Fact]
    public async void GetWithDetailsAsync_NotExistingKey_ThrowsNotFoundException()
    {
        const string gameKey = "not-existing-game-key";

        _gameServiceMock.Setup(service => service.GetByKeyAsync(gameKey))
                        .ThrowsAsync(new ItemNotFoundException());
        
        var operation = async () => await _gameController.GetWithDetailsAsync(gameKey);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }
    
    [Fact]
    public async void GetFileAsync_ExistingKey_ReturnsGame()
    {
        var expectedResult = new byte[] { 0, 0, 0, 0 };
        const string gameKey = "existing-game-key";

        _gameServiceMock.Setup(service => service.GetFileAsync(gameKey))
                        .ReturnsAsync(expectedResult);
        
        var actualResult = await _gameController.GetFileAsync(gameKey);
        
        Assert.Equal(expectedResult, (actualResult.Result as OkObjectResult)?.Value);
    }
    
    [Fact]
    public async void GetFileAsync_NotExistingKey_ThrowsNotFoundException()
    {
        const string gameKey = "not-existing-game-key";

        _gameServiceMock.Setup(service => service.GetFileAsync(gameKey))
                        .ThrowsAsync(new ItemNotFoundException());
        
        var operation = async () => await _gameController.GetFileAsync(gameKey);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }

    [Fact]
    public async void CreateAsync_CorrectValue_ReturnsModel()
    {
        var expectedId = Guid.NewGuid();
        var createModel = new GameCreateRequestModel();

        _mapperMock.Setup(mapper => mapper.Map<GameCreateModel>(It.IsAny<GameCreateRequestModel>()))
                   .Returns(new GameCreateModel());
        _mapperMock.Setup(mapper => mapper.Map<GameViewModel>(It.Is<Game>(game => game.Id == expectedId)))
                   .Returns(new GameViewModel { Id = expectedId });
        _gameServiceMock.Setup(service => service.CreateAsync(It.IsAny<GameCreateModel>()))
                        .ReturnsAsync(new Game { Id = expectedId });
        
        var actualResult = await _gameController.CreateAsync(createModel);
        
        var actualObjectResult = Assert.IsType<OkObjectResult>(actualResult.Result);
        var actualResultModel = Assert.IsType<GameViewModel>(actualObjectResult.Value);
        Assert.Equal(expectedId, actualResultModel.Id);
    }
    
    [Fact]
    public async void CommentGameAsync_CorrectValue()
    {
        var createModel = new CommentCreateRequestModel();

        _mapperMock.Setup(mapper => mapper.Map<CommentCreateModel>(It.IsAny<CommentCreateRequestModel>()))
                   .Returns(new CommentCreateModel());
        
        var actualResult = await _gameController.CommentGameAsync(createModel);

        Assert.IsType<OkResult>(actualResult);
        _commentServiceMock.Verify(service => service.CommentGameAsync(It.IsAny<CommentCreateModel>()), Times.Once);
    }

    [Fact]
    public async void UpdateAsync_CorrectValues()
    {
        var editModel = new GameEditRequestModel();

        _mapperMock.Setup(mapper => mapper.Map<GameUpdateModel>(It.IsAny<GameEditRequestModel>()))
                   .Returns(new GameUpdateModel());

        var actualResult = await _gameController.UpdateAsync(editModel);

        Assert.IsType<OkResult>(actualResult);
        _gameServiceMock.Verify(service => service.UpdateAsync(It.IsAny<GameUpdateModel>()), Times.Once);
    }

    [Fact]
    public async void DeleteAsync_CorrectValues()
    {
        var id = Guid.NewGuid();
        
        var actualResult = await _gameController.DeleteAsync(id);

        Assert.IsType<OkResult>(actualResult);
        _gameServiceMock.Verify(service => service.DeleteAsync(id));
    }
}