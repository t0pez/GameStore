using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Exceptions;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.Games.Specifications;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using GameStore.Tests.Infrastructure.Attributes;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services.GameServiceTests;

public partial class GameServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task GetFileAsync_ExistingKey_ReturnsFile(
        Game game,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        GameService sut)
    {
        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Game>().GetSingleOrDefaultBySpecAsync(
                                     It.IsAny<GamesSpec>()))
                      .ReturnsAsync(game);

        var actualResult = await sut.GetFileAsync(game.Key);

        actualResult.Should().BeEquivalentTo(game.File);
    }

    [Theory]
    [AutoMoqData]
    public async Task GetFileAsync_NotExistingKey_ThrowsNotFoundException(
        string gameKey,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        GameService sut)
    {
        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Game>().GetSingleOrDefaultBySpecAsync(
                                     It.IsAny<GamesSpec>()))
                      .ReturnsAsync(() => null!);

        var operation = async () => await sut.GetFileAsync(gameKey);

        await operation.Should().ThrowAsync<ItemNotFoundException>();
    }
}