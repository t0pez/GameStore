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
    public async Task GetByKeyAsync_ExistingKey_ReturnsGames(
        Game game,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        GameService sut)
    {
        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Game>().GetSingleOrDefaultBySpecAsync(
                                     It.IsAny<GamesSpec>()))
                      .ReturnsAsync(game);

        var actualResult = await sut.GetByKeyAsync(game.Key);

        actualResult.Should().Be(game);
    }

    [Theory]
    [AutoMoqData]
    public async Task GetByKeyAsync_NotExistingKey_ThrowsNotFoundException(
        string gameKey,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        GameService sut)
    {
        unitOfWorkMock
           .Setup(repository => repository.GetEfRepository<Game>()
                                          .GetSingleOrDefaultBySpecAsync(
                                               It.IsAny<GamesSpec>()))
           .ReturnsAsync(() => null!);

        var operation = async () => await sut.GetByKeyAsync(gameKey);

        await operation.Should().ThrowAsync<ItemNotFoundException>();
    }
}