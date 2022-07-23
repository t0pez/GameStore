using System.Collections.Generic;
using System.IO;
using AutoMapper;
using FluentAssertions;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications.Filters;
using GameStore.Core.Models.Genres;
using GameStore.Core.Models.PlatformTypes;
using GameStore.Core.Models.ServiceModels.Enums;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.Core.PagedResult;
using GameStore.SharedKernel.Specifications.Filters;
using GameStore.Web.Controllers;
using GameStore.Web.Interfaces;
using GameStore.Web.Models.Game;
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
    private readonly Mock<IGenreService> _genreServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IPlatformTypeService> _platformServiceMock;
    private readonly Mock<IPublisherService> _publisherServiceMock;
    private readonly Mock<ISearchService> _searchServiceMock;
    private readonly Mock<IUserCookieService> _userCookieServiceMock;

    public GameControllerTests()
    {
        var orderServiceMock = new Mock<IOrderService>();
        _userCookieServiceMock = new Mock<IUserCookieService>();
        _searchServiceMock = new Mock<ISearchService>();
        _gameServiceMock = new Mock<IGameService>();
        _publisherServiceMock = new Mock<IPublisherService>();
        _genreServiceMock = new Mock<IGenreService>();
        _platformServiceMock = new Mock<IPlatformTypeService>();
        _mapperMock = new Mock<IMapper>();

        _gameController = new GamesController(_searchServiceMock.Object, _gameServiceMock.Object,
                                              _publisherServiceMock.Object, _genreServiceMock.Object,
                                              _platformServiceMock.Object, _mapperMock.Object, orderServiceMock.Object,
                                              _userCookieServiceMock.Object);
        _gameController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
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
    public async void CreateAsync_NoParameters_ReturnsView()
    {
        _genreServiceMock.Setup(service => service.GetAllAsync())
                         .ReturnsAsync(new List<Genre>());
        _platformServiceMock.Setup(service => service.GetAllAsync())
                            .ReturnsAsync(new List<PlatformType>());
        _publisherServiceMock.Setup(service => service.GetAllAsync())
                             .ReturnsAsync(new List<PublisherDto>());

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
        _gameController.ControllerContext.HttpContext.Request.Form =
            new FormCollection(new Dictionary<string, StringValues>(), fileCollection);

        _mapperMock.Setup(mapper => mapper.Map<GameCreateModel>(requestModel))
                   .Returns(createModel);
        _gameServiceMock.Setup(service => service.CreateAsync(createModel))
                        .ReturnsAsync(new Game { Key = gameKey });

        var actualResult = await _gameController.CreateAsync(requestModel);

        actualResult.Should().BeAssignableTo<RedirectToActionResult>();
    }

    [Theory]
    [InlineData("Some name", "{ key = some-name }")]
    [InlineData("First game Part 2", "{ key = first-game-part-2 }")]
    public async void GenerateKeyAsync_GameKeyAsParameter_GeneratesCorrectKeys(string name, string expectedJsonValue)
    {
        var actualResult = await _gameController.GenerateKeyAsync(name);

        var actualJsonResult = Assert.IsType<JsonResult>(actualResult);
        Assert.Equal(expectedJsonValue, actualJsonResult.Value.ToString());
    }

    [Fact]
    public async void GetWithDetailsAsync_ExistingKey_ReturnsGameView()
    {
        const string gameKey = "existing-game-key";

        _searchServiceMock.Setup(service => service.GetProductDtoByGameKeyOrDefaultAsync(gameKey))
                          .ReturnsAsync(new ProductDto { Key = gameKey });
        _mapperMock.Setup(mapper => mapper.Map<GameViewModel>(It.IsAny<ProductDto>()))
                   .Returns(new GameViewModel { Key = gameKey });
        string a = null!;
        _userCookieServiceMock
            .Setup(service => service.TryGetCookiesUserId(It.IsAny<IRequestCookieCollection>(), out a))
            .Returns(false);

        var actualResult = await _gameController.GetWithDetailsAsync(gameKey);

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<GameViewModel>()
                    .Which.Key.Should().Be(gameKey);
    }

    [Fact]
    public async void GetWithDetailsAsync_NotExistingKey_ThrowsNotFoundException()
    {
        const string gameKey = "not-existing-game-key";

        _searchServiceMock.Setup(service => service.GetProductDtoByGameKeyOrDefaultAsync(gameKey))
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
    public async void UpdateAsync_NoParameters_ReturnsView()
    {
        const string gameKey = "game-key";

        _genreServiceMock.Setup(service => service.GetAllAsync())
                         .ReturnsAsync(new List<Genre>());
        _platformServiceMock.Setup(service => service.GetAllAsync())
                            .ReturnsAsync(new List<PlatformType>());
        _publisherServiceMock.Setup(service => service.GetAllAsync())
                             .ReturnsAsync(new List<PublisherDto>());


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
        _gameController.ControllerContext.HttpContext.Request.Form =
            new FormCollection(new Dictionary<string, StringValues>(), fileCollection);

        var updateRequestModel = new GameUpdateRequestModel();

        var actualResult = await _gameController.UpdateAsync(updateRequestModel, "");

        Assert.IsType<RedirectToActionResult>(actualResult);
        _gameServiceMock.Verify(service => service.UpdateFromEndpointAsync(It.IsAny<GameUpdateModel>()), Times.Once);
    }

    [Fact]
    public async void DeleteAsync_CorrectValues_ReturnsRedirect()
    {
        const string gameKey = "game-key";
        const Database database = Database.Server;

        var actualResult = await _gameController.DeleteAsync(gameKey, (int)database);

        Assert.IsType<RedirectToActionResult>(actualResult);
        _gameServiceMock.Verify(service => service.DeleteAsync(gameKey, database));
    }
}