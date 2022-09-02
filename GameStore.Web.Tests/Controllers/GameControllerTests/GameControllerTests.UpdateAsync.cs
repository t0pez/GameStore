using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Server.Genres;
using GameStore.Core.Models.Server.PlatformTypes;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using GameStore.Web.Interfaces;
using GameStore.Web.Models.Game;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.GameControllerTests;

public partial class GameControllerTests
{
    [Theory]
    [AutoMoqData]
    public async Task UpdateAsync_NoParameters_ReturnsView(
        ProductDto product,
        [Frozen] Mock<ISearchService> searchServiceMock,
        [Frozen] Mock<IGenreService> genreServiceMock,
        [Frozen] Mock<IPlatformTypeService> platformServiceMock,
        [Frozen] Mock<IPublisherService> publisherServiceMock,
        [Frozen] Mock<IPublisherAuthHelper> publisherAuthMock,
        GamesController sut)
    {
        searchServiceMock.Setup(service => service.GetProductDtoByGameKeyOrDefaultAsync(product.Key))
                         .ReturnsAsync(product);

        publisherAuthMock
           .Setup(helper => helper.CanEditAsync(It.IsAny<string>()))
           .ReturnsAsync(true);

        genreServiceMock.Setup(service => service.GetAllAsync())
                        .ReturnsAsync(new List<Genre>());

        platformServiceMock.Setup(service => service.GetAllAsync())
                           .ReturnsAsync(new List<PlatformType>());

        publisherServiceMock.Setup(service => service.GetAllAsync())
                            .ReturnsAsync(new List<PublisherDto>());

        var actualResult = await sut.UpdateAsync(product.Key);

        actualResult.Should().BeAssignableTo<ViewResult>();
    }

    [Theory]
    [AutoMoqData]
    public async void UpdateAsync_CorrectValues_ReturnsRedirect(
        GameUpdateRequestModel requestModel,
        ProductDto product,
        [Frozen] Mock<ISearchService> searchServiceMock,
        [Frozen] Mock<IGameService> gameServiceMock,
        [Frozen] Mock<IPublisherAuthHelper> publisherAuthMock,
        GamesController sut)
    {
        searchServiceMock.Setup(service => service.GetProductDtoByGameKeyOrDefaultAsync(product.Key))
                         .ReturnsAsync(product);

        publisherAuthMock
           .Setup(helper => helper.CanEditAsync(It.IsAny<string>()))
           .ReturnsAsync(true);

        var actualResult = await sut.UpdateAsync(requestModel, product.Key);

        actualResult.Should().BeOfType<RedirectToActionResult>();

        gameServiceMock.Verify(service => service.UpdateFromEndpointAsync(It.IsAny<GameUpdateModel>()), Times.Once);
    }
}