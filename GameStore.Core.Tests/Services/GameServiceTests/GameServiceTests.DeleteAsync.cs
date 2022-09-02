using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.Games.Specifications;
using GameStore.Core.Models.ServiceModels.Enums;
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
    public async Task DeleteAsync_ServerEntity_GameSoftDeleted(
        string gameKey,
        ProductDto dto,
        Game game,
        [Frozen] Mock<ISearchService> searchServiceMock,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        GameService sut)
    {
        dto.Database = Database.Server;
        dto.Key = gameKey;
        game.Key = gameKey;

        searchServiceMock.Setup(service => service.GetProductDtoByGameKeyOrDefaultAsync(gameKey))
                         .ReturnsAsync(dto);

        unitOfWorkMock
           .Setup(repository => repository.GetEfRepository<Game>()
                                          .GetSingleOrDefaultBySpecAsync(
                                               It.IsAny<GamesSpec>()))
           .ReturnsAsync(game);

        await sut.DeleteAsync(gameKey, Database.Server);

        unitOfWorkMock.Verify(
            repository => repository.GetEfRepository<Game>()
                                    .UpdateAsync(It.Is<Game>(game => game.Key == gameKey && game.IsDeleted == true)),
            Times.Once);

        unitOfWorkMock.Verify(work => work.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task DeleteAsync_NotCorrectValues_ThrowsNotFoundException(
        string gameKey,
        [Frozen] Mock<ISearchService> searchServiceMock,
        GameService sut)
    {
        searchServiceMock.Setup(service =>
                                    service.GetProductDtoByGameKeyOrDefaultAsync(gameKey))
                         .ReturnsAsync(() => null!);

        var operation = async () => await sut.DeleteAsync(gameKey, Database.Server);

        await operation.Should().ThrowAsync<ItemNotFoundException>();
    }
}