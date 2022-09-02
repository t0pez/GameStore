using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
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
    public async Task GetFileAsync_ExistingKey_ReturnsFile(
        string gameKey,
        byte[] file,
        [Frozen] Mock<IGameService> gameServiceMock,
        GamesController sut)
    {
        gameServiceMock.Setup(service => service.GetFileAsync(gameKey))
                       .ReturnsAsync(file);

        var actualResult = await sut.GetFileAsync(gameKey);

        actualResult.Should().BeOfType<FileContentResult>()
                    .Which.FileContents.Should().BeEquivalentTo(file);
    }

    [Theory]
    [AutoMoqData]
    public async Task GetFileAsync_NotExistingKey_ThrowsNotFoundException(
        string gameKey,
        [Frozen] Mock<IGameService> gameServiceMock,
        GamesController sut)
    {
        gameServiceMock.Setup(service => service.GetFileAsync(gameKey))
                       .ThrowsAsync(new ItemNotFoundException());

        var operation = async () => await sut.GetFileAsync(gameKey);

        await operation.Should().ThrowAsync<ItemNotFoundException>();
    }
}