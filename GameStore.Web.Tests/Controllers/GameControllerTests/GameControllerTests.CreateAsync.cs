using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.Genres;
using GameStore.Core.Models.Server.PlatformTypes;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using GameStore.Web.Models.Game;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.GameControllerTests;

public partial class GameControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void CreateAsync_NoParameters_ReturnsView(
        List<Genre> genres,
        List<PlatformType> platformTypes,
        List<PublisherDto> publishers,
        [Frozen] Mock<IGenreService> genreServiceMock,
        [Frozen] Mock<IPlatformTypeService> platformServiceMock,
        [Frozen] Mock<IPublisherService> publisherServiceMock,
        GamesController sut)
    {
        genreServiceMock.Setup(service => service.GetAllAsync())
                        .ReturnsAsync(genres);

        platformServiceMock.Setup(service => service.GetAllAsync())
                           .ReturnsAsync(platformTypes);

        publisherServiceMock.Setup(service => service.GetAllAsync())
                            .ReturnsAsync(publishers);

        var actualResult = await sut.CreateAsync();

        actualResult.Should().BeAssignableTo<ViewResult>();
    }

    [Theory]
    [AutoMoqData]
    public async void CreateAsync_CorrectParameters_ReturnsRedirect(
        GameCreateRequestModel requestModel,
        Game game,
        [Frozen] Mock<IGameService> gameServiceMock,
        GamesController sut)
    {
        gameServiceMock.Setup(service => service.CreateAsync(It.IsAny<GameCreateModel>()))
                       .ReturnsAsync(game);

        var actualResult = await sut.CreateAsync(requestModel);

        actualResult.Should().BeAssignableTo<RedirectToActionResult>();
    }
}