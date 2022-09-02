using System.Threading.Tasks;
using AutoFixture.Xunit2;
using GameStore.Core.Models.Server.Comments;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.Games.Specifications;
using GameStore.Core.Models.ServiceModels.Comments;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using GameStore.Tests.Infrastructure.Attributes;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services.CommentServiceTests;

public partial class CommentServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task CommentGameAsync_ExistingKey(
        CommentCreateModel createModel,
        Game game,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        CommentService sut)
    {
        unitOfWorkMock
           .Setup(repository => repository.GetEfRepository<Game>()
                                          .GetSingleOrDefaultBySpecAsync(It.IsAny<GamesSpec>()))
           .ReturnsAsync(game);

        await sut.CommentGameAsync(createModel);

        unitOfWorkMock.Verify(repository => repository.GetEfRepository<Comment>().AddAsync(It.IsAny<Comment>()),
                              Times.Once());

        unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(), Times.Once);
    }
}