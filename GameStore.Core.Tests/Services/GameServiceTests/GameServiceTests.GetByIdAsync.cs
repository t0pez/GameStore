using System;
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
    public async Task GetByIdAsync_ExistingId_ReturnsGames(
        Game game,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        GameService sut)
    {
        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Game>().GetSingleOrDefaultBySpecAsync(
                                     It.IsAny<GamesSpec>()))
                      .ReturnsAsync(game);

        var actualResult = await sut.GetByIdAsync(game.Id);

        actualResult.Id.Should().Be(game.Id);
    }

    [Theory]
    [AutoMoqData]
    public async Task GetByIdAsync_NotExistingId_ThrowsNotFoundException(
        Guid gameId,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        GameService sut)
    {
        unitOfWorkMock
           .Setup(repository =>
                      repository.GetEfRepository<Game>().GetSingleOrDefaultBySpecAsync(
                          It.IsAny<GamesSpec>()))
           .ReturnsAsync(() => null!);

        var operation = async () => await sut.GetByIdAsync(gameId);

        await operation.Should().ThrowAsync<ItemNotFoundException>();
    }
}