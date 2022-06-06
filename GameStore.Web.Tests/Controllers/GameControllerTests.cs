using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using FluentAssertions;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications.Filters;
using GameStore.Core.Models.Genres;
using GameStore.Core.Models.PlatformTypes;
using GameStore.Core.Models.Publishers;
using GameStore.Core.Models.ServiceModels.Comments;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.Core.PagedResult;
using GameStore.SharedKernel.Specifications.Filters;
using GameStore.Web.Controllers;
using GameStore.Web.Models.Comment;
using GameStore.Web.Models.Game;
using GameStore.Web.ViewModels.Comments;
using GameStore.Web.ViewModels.Games;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers;

public class GameControllerTests
{
    private readonly GamesController _gameController;
    private readonly Mock<IGameService> _gameServiceMock;
    private readonly Mock<ICommentService> _commentServiceMock;
    private readonly Mock<IPublisherService> _publisherServiceMock;
    private readonly Mock<IGenreService> _genreServiceMock;
    private readonly Mock<IPlatformTypeService> _platformServiceMock;
    private readonly Mock<IMapper> _mapperMock;

    public GameControllerTests()
    {
        _gameServiceMock = new Mock<IGameService>();
        _commentServiceMock = new Mock<ICommentService>();
        _publisherServiceMock = new Mock<IPublisherService>();
        _genreServiceMock = new Mock<IGenreService>();
        _platformServiceMock = new Mock<IPlatformTypeService>();
        _mapperMock = new Mock<IMapper>();

        _gameController = new GamesController(_gameServiceMock.Object, _commentServiceMock.Object, _publisherServiceMock.Object,
                                              _genreServiceMock.Object, _platformServiceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async void GetTotalGamesCountAsync_NoParameters_ReturnsCorrectValue()
    {
        const int expectedGamesCount = 5;

        _gameServiceMock.Setup(service => service.GetTotalCountAsync())
                        .ReturnsAsync(5);

        var actualResult = await _gameController.GetTotalGamesCount();
        Assert.Equal(expectedGamesCount, actualResult);
    }

    [Fact]
    public async void GetAllAsync_EmptyFilter_CreatesDefaultFilter()
    {
        GamesFilterRequestModel filterRequestModel = null!;
        int? currentPage = null;
        int? pageSize = null;

        _mapperMock.Setup(mapper => mapper.Map<GameSearchFilter>(filterRequestModel))
                   .Returns(new GameSearchFilter { CurrentPage = 1, PageSize = 10 });
        _gameServiceMock.Setup(service => service.GetByFilterAsync(It.IsAny<GameSearchFilter>()))
                        .ReturnsAsync(new PagedResult<Game>(new List<Game>(), 1,
                                                            new PaginationFilter { CurrentPage = 1, PageSize = 10 }));
        _genreServiceMock.Setup(service => service.GetAllAsync())
                         .ReturnsAsync(new List<Genre>());
        _platformServiceMock.Setup(service => service.GetAllAsync())
                            .ReturnsAsync(new List<PlatformType>());
        _publisherServiceMock.Setup(service => service.GetAllAsync())
                            .ReturnsAsync(new List<Publisher>());

        var actualResult = await _gameController.GetAllAsync(filterRequestModel, currentPage, pageSize);

        actualResult.Result.Should().BeAssignableTo<ViewResult>();
    }

    [Fact]
    public async void CreateAsync_NoParameters_ReturnsView()
    {
        _genreServiceMock.Setup(service => service.GetAllAsync())
                         .ReturnsAsync(new List<Genre>());
        _platformServiceMock.Setup(service => service.GetAllAsync())
                            .ReturnsAsync(new List<PlatformType>());
        _publisherServiceMock.Setup(service => service.GetAllAsync())
                             .ReturnsAsync(new List<Publisher>());
        
        var actualResult = await _gameController.CreateAsync();

        actualResult.Should().BeAssignableTo<ViewResult>();
    }
    
    [Fact]
    public async void CreateAsync_CorrectParameters_ReturnsRedirect()
    {
        const string gameKey = "game-key";
        var requestModel = new GameCreateRequestModel
        {
            Key = gameKey
        };
        var createModel = new GameCreateModel
        {
            Key = gameKey
        };
        
        var fileCollection = new FormFileCollection { new FormFile(Stream.Null, 0, 0, "", "") };
        _gameController.ControllerContext = new ControllerContext();
        _gameController.ControllerContext.HttpContext = new DefaultHttpContext();
        _gameController.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), fileCollection);

        _mapperMock.Setup(mapper => mapper.Map<GameCreateModel>(requestModel))
                   .Returns(createModel);
        _gameServiceMock.Setup(service => service.CreateAsync(createModel))
                        .ReturnsAsync(new Game { Key = gameKey });

        var actualResult = await _gameController.CreateAsync(requestModel);

        actualResult.Should().BeAssignableTo<RedirectToActionResult>();
    }

    [Fact]
    public async void GetCommentsAsync_ExistingGameKey_ReturnsCommentsView()
    {
        const int expectedCommentsCount = 5;

        _commentServiceMock.Setup(service => service.GetCommentsByGameKeyAsync(It.IsAny<string>()))
                           .ReturnsAsync(new List<Comment>(new Comment[expectedCommentsCount]));

        var actualResult = await _gameController.GetCommentsAsync("");
        Assert.IsType<ActionResult<ICollection<CommentViewModel>>>(actualResult);
    }

    [Theory]
    [InlineData("Some name", "{ key = some-name }")]
    [InlineData("First game. Part 2", "{ key = first-game-part-2 }")]
    public async void GenerateKeyAsync_GameKeyAsParameter_GeneratesCorrectKeys(string name, string expectedJsonValue)
    {
        var actualResult = await _gameController.GenerateKeyAsync(name);

        var actualJsonResult = Assert.IsType<JsonResult>(actualResult);
        Assert.Equal(expectedJsonValue, actualJsonResult.Value.ToString());
    }
    
    [Fact]
    public async void GetWithDetailsAsync_ExistingKey_ReturnsGameView()
    {
        var expectedId = Guid.NewGuid();
        const string gameKey = "existing-game-key";

        _gameServiceMock.Setup(service => service.GetByKeyAsync(gameKey))
                        .ReturnsAsync(new Game {Id = expectedId});
        _mapperMock.Setup(mapper => mapper.Map<GameViewModel>(It.IsAny<Game>()))
                   .Returns(new GameViewModel { Id = expectedId });

        var actualResult = await _gameController.GetWithDetailsAsync(gameKey);

        var actualViewResult = Assert.IsType<ViewResult>(actualResult.Result);
        var actualResultModel = Assert.IsType<GameViewModel>(actualViewResult.Model);
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
    public async void GetFileAsync_ExistingKey_ReturnsFile()
    {
        var expectedResult = new byte[] { 0, 0, 0, 0 };
        const string gameKey = "existing-game-key";

        _gameServiceMock.Setup(service => service.GetFileAsync(gameKey))
                        .ReturnsAsync(expectedResult);
        
        var actualResult = await _gameController.GetFileAsync(gameKey);
        var actualFileResult = Assert.IsType<FileContentResult>(actualResult);
        Assert.Equal(expectedResult, actualFileResult.FileContents);
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
    public async void CommentGameAsync_CorrectValue_ReturnsRedirect()
    {
        var createModel = new CommentCreateRequestModel();

        _mapperMock.Setup(mapper => mapper.Map<CommentCreateModel>(It.IsAny<CommentCreateRequestModel>()))
                   .Returns(new CommentCreateModel());
        
        var actualResult = await _gameController.CreateCommentAsync(createModel);

        Assert.IsType<RedirectToActionResult>(actualResult);
        _commentServiceMock.Verify(service => service.CommentGameAsync(It.IsAny<CommentCreateModel>()), Times.Once);
    }
    
    [Fact]
    public async void UpdateCommentAsync_CorrectValue_ReturnsRedirect()
    {
        var updateRequest = new CommentUpdateRequestModel();
        var updateModel = new CommentUpdateModel();

        _mapperMock.Setup(mapper => mapper.Map<CommentUpdateModel>(updateRequest))
                   .Returns(updateModel);
        
        var actualResult = await _gameController.UpdateCommentAsync(updateRequest);

        actualResult.Should().BeAssignableTo<RedirectToActionResult>();
        _commentServiceMock.Verify(service => service.UpdateAsync(updateModel), Times.Once);
    }
    
    [Fact]
    public async void DeleteCommentAsync_CorrectValue_ReturnsRedirect()
    {
        var commentId = Guid.NewGuid();
        const string gameKey = "game-key";
        
        var actualResult = await _gameController.DeleteCommentAsync(commentId, gameKey);

        actualResult.Should().BeAssignableTo<RedirectToActionResult>();
        _commentServiceMock.Verify(service => service.DeleteAsync(commentId), Times.Once);
    }
    
    [Fact]
    public async void DeleteCommentAsync_NotCorrectValue_ThrowsException()
    {
        var commentId = Guid.NewGuid();
        const string gameKey = "game-key";
        
        var actualResult = await _gameController.DeleteCommentAsync(commentId, gameKey);

        actualResult.Should().BeAssignableTo<RedirectToActionResult>();
        _commentServiceMock.Verify(service => service.DeleteAsync(commentId), Times.Once);
    }

    [Fact]
    public async void UpdateAsync_NoParameters_ReturnsView()
    {
        const string gameKey = "game-key";
        
        _genreServiceMock.Setup(service => service.GetAllAsync())
                         .ReturnsAsync(new List<Genre>());
        _platformServiceMock.Setup(service => service.GetAllAsync())
                            .ReturnsAsync(new List<PlatformType>());
        _publisherServiceMock.Setup(service => service.GetAllAsync())
                             .ReturnsAsync(new List<Publisher>());

        
        var actualResult = await _gameController.UpdateAsync(gameKey);

        actualResult.Should().BeAssignableTo<ViewResult>();
    }
    
    [Fact]
    public async void UpdateAsync_CorrectValues_ReturnsRedirect()
    {
        _mapperMock.Setup(mapper => mapper.Map<GameUpdateModel>(It.IsAny<GameUpdateRequestModel>()))
                   .Returns(new GameUpdateModel());

        var fileCollection = new FormFileCollection { new FormFile(Stream.Null, 0, 0, "", "") };
        _gameController.ControllerContext = new ControllerContext();
        _gameController.ControllerContext.HttpContext = new DefaultHttpContext();
        _gameController.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), fileCollection);

        var updateRequestModel = new GameUpdateRequestModel();

        var actualResult = await _gameController.UpdateAsync(updateRequestModel);

        Assert.IsType<RedirectToActionResult>(actualResult);
        _gameServiceMock.Verify(service => service.UpdateAsync(It.IsAny<GameUpdateModel>()), Times.Once);
    }

    [Fact]
    public async void DeleteAsync_CorrectValues_ReturnsRedirect()
    {
        var id = Guid.NewGuid();
        
        var actualResult = await _gameController.DeleteAsync(id);

        Assert.IsType<RedirectToActionResult>(actualResult);
        _gameServiceMock.Verify(service => service.DeleteAsync(id));
    }
}