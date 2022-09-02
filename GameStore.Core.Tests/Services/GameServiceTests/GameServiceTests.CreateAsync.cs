using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.Games.Specifications;
using GameStore.Core.Models.ServiceModels.Games;
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
    public async Task CreateAsync_CorrectValues_ReturnsGame(
        GameCreateModel createModel,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        GameService sut)
    {
        unitOfWorkMock
           .Setup(repository => repository.GetEfRepository<Game>()
                                          .AnyAsync(It.IsAny<GamesSpec>()))
           .ReturnsAsync(false);

        var actualResult = await sut.CreateAsync(createModel);

        actualResult.Key.Should().Be(createModel.Key);

        unitOfWorkMock.Verify(repository => repository.GetEfRepository<Game>().AddAsync(It.IsAny<Game>()), Times.Once);
        unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(), Times.Once);
    }
}