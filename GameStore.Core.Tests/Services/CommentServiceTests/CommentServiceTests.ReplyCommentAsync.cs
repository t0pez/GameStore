using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Exceptions;
using GameStore.Core.Models.Server.Comments;
using GameStore.Core.Models.Server.Comments.Specifications;
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
    public async Task ReplyCommentAsync_CorrectValues(
        ReplyCreateModel createModel,
        Game game,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        CommentService sut)
    {
        unitOfWorkMock
           .Setup(repository => repository.GetEfRepository<Game>()
                                          .GetSingleOrDefaultBySpecAsync(
                                               It.IsAny<GamesSpec>()))
           .ReturnsAsync(game);

        unitOfWorkMock
           .Setup(repository => repository.GetEfRepository<Comment>()
                                          .AnyAsync(It.IsAny<CommentsSpec>()))
           .ReturnsAsync(true);

        await sut.ReplyCommentAsync(createModel);

        unitOfWorkMock.Verify(repository => repository.GetEfRepository<Comment>().AddAsync(It.IsAny<Comment>()),
                              Times.Once);

        unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async void ReplyCommentAsync_NotExistingParentComment_ThrowsNotFoundException(
        ReplyCreateModel createModel,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        CommentService sut)
    {
        unitOfWorkMock.Setup(repository => repository.GetEfRepository<Comment>().AnyAsync(It.IsAny<CommentsSpec>()))
                      .ReturnsAsync(false);

        var operation = async () => await sut.ReplyCommentAsync(createModel);

        await operation.Should().ThrowAsync<ItemNotFoundException>();
    }

    [Theory]
    [AutoMoqData]
    public async void ReplyCommentAsync_NotExistingGame_ThrowsNotFoundException(
        ReplyCreateModel createModel,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        CommentService sut)
    {
        unitOfWorkMock.Setup(repository => repository.GetEfRepository<Comment>().AnyAsync(It.IsAny<CommentsSpec>()))
                      .ReturnsAsync(true);

        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Game>()
                                           .GetSingleOrDefaultBySpecAsync(
                                                It.IsAny<GamesSpec>()))
                      .ReturnsAsync(() => null!);

        var operation = async () => await sut.ReplyCommentAsync(createModel);

        await operation.Should().ThrowAsync<ItemNotFoundException>();
    }
}