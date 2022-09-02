using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.ServiceModels.Enums;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.GameControllerTests;

public partial class GameControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void DeleteAsync_CorrectValues_ReturnsRedirect(
        string gameKey,
        [Frozen] Mock<IGameService> gameServiceMock,
        GamesController sut)
    {
        const Database database = Database.Server;

        var actualResult = await sut.DeleteAsync(gameKey, (int)database);

        actualResult.Should().BeOfType<RedirectToActionResult>();

        gameServiceMock.Verify(service => service.DeleteAsync(gameKey, database));
    }
}