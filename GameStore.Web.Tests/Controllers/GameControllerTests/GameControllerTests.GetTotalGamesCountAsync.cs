using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.GameControllerTests;

public partial class GameControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void GetTotalGamesCountAsync_NoParameters_ReturnsCorrectValue(
        int count,
        [Frozen] Mock<IGameService> gameServiceMock,
        GamesController sut)
    {
        gameServiceMock.Setup(service => service.GetTotalCountAsync())
                       .ReturnsAsync(count);

        var actualResult = await sut.GetTotalGamesCount();

        actualResult.Should().Be(count);
    }
}