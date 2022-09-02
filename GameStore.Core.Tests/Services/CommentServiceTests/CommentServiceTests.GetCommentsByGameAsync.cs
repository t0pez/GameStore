using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Exceptions;
using GameStore.Core.Models.Server.Comments;
using GameStore.Core.Models.Server.Comments.Specifications;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.Games.Specifications;
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
    public async Task GetCommentsByGameAsync_ExistingKey_ReturnsComments(
        List<Comment> comments,
        string gameKey,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        CommentService sut)
    {
        unitOfWorkMock.Setup(unitOfWork => unitOfWork.GetEfRepository<Game>().AnyAsync(It.IsAny<GamesSpec>()))
                      .ReturnsAsync(true);

        unitOfWorkMock.Setup(unitOfWork => unitOfWork.GetEfRepository<Comment>()
                                                     .GetBySpecAsync(It.IsAny<CommentsSpec>()))
                      .ReturnsAsync(comments);

        var actualResult = await sut.GetCommentsByGameKeyAsync(gameKey);

        actualResult.Count.Should().Be(comments.Count);
    }

    [Theory]
    [AutoMoqData]
    public async void GetCommentsByGameAsync_NotExistingKey_ThrowsNotFoundException(
        string gameKey,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        CommentService sut)
    {
        unitOfWorkMock
           .Setup(repository =>
                      repository.GetEfRepository<Game>().AnyAsync(It.IsAny<GamesSpec>()))
           .ReturnsAsync(false);

        var operation = async () => await sut.GetCommentsByGameKeyAsync(gameKey);

        await operation.Should().ThrowAsync<ItemNotFoundException>();
    }
}