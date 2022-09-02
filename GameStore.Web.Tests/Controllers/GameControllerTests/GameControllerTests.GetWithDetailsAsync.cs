using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Dto;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using GameStore.Web.Interfaces;
using GameStore.Web.ViewModels.Games;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.GameControllerTests;

public partial class GameControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void GetWithDetailsAsync_ExistingKey_ReturnsGameView(
        Guid userCookieId,
        ProductDto product,
        [Frozen] Mock<ISearchService> searchServiceMock,
        [Frozen] Mock<IUserCookieService> userCookieServiceMock,
        GamesController sut)
    {
        searchServiceMock.Setup(service => service.GetProductDtoByGameKeyOrDefaultAsync(product.Key))
                         .ReturnsAsync(product);

        userCookieServiceMock
           .Setup(service => service.GetCookiesUserId())
           .Returns(userCookieId);

        var actualResult = await sut.GetWithDetailsAsync(product.Key);

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<GameViewModel>()
                    .Which.Key.Should().Be(product.Key);
    }

    [Theory]
    [AutoMoqData]
    public async void GetWithDetailsAsync_NotExistingKey_ThrowsNotFoundException(
        [Frozen] Mock<ISearchService> searchServiceMock,
        GamesController sut)
    {
        const string gameKey = "not-existing-game-key";

        searchServiceMock.Setup(service => service.GetProductDtoByGameKeyOrDefaultAsync(gameKey))
                         .ThrowsAsync(new ItemNotFoundException());

        var operation = async () => await sut.GetWithDetailsAsync(gameKey);

        await operation.Should().ThrowAsync<ItemNotFoundException>();
    }
}