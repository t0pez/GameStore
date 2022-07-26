using System;
using System.Collections.Generic;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.Loggers;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Comments.Specifications;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.Core.Models.ServiceModels.Comments;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services;

public class CommentServiceTests
{
    private readonly Mock<IRepository<Comment>> _commentRepoMock;
    private readonly ICommentService _commentService;
    private readonly Mock<IRepository<Game>> _gameRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;


    public CommentServiceTests()
    {
        var loggerMock = new Mock<ILogger<CommentService>>();
        var mongoLoggerMock = new Mock<IMongoLogger>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _gameRepoMock = new Mock<IRepository<Game>>();
        _commentRepoMock = new Mock<IRepository<Comment>>();
        _mapperMock = new Mock<IMapper>();

        _unitOfWorkMock.Setup(unit => unit.GetEfRepository<Game>())
                       .Returns(_gameRepoMock.Object);
        _unitOfWorkMock.Setup(unit => unit.GetEfRepository<Comment>())
                       .Returns(_commentRepoMock.Object);

        _commentService = new CommentService(loggerMock.Object, mongoLoggerMock.Object, _unitOfWorkMock.Object,
                                             _mapperMock.Object);
    }

    [Fact]
    public async void GetCommentsByGameAsync_ExistingKey_ReturnsComments()
    {
        const int expectedResultCount = 3;
        const string gameKey = "existing-game-key";

        _gameRepoMock.Setup(repository => repository.AnyAsync(It.Is<GameByKeySpec>(spec => spec.Key == gameKey)))
                     .ReturnsAsync(true);
        _commentRepoMock.Setup(repository =>
                                   repository.GetBySpecAsync(
                                       It.Is<CommentsWithoutParentByGameKeySpec>(spec => spec.GameKey == gameKey)))
                        .ReturnsAsync(new List<Comment>(new Comment[expectedResultCount]));

        var actualResult = await _commentService.GetCommentsByGameKeyAsync(gameKey);
        var actualResultCount = actualResult.Count;

        Assert.Equal(expectedResultCount, actualResultCount);
    }

    [Fact]
    public async void GetCommentsByGameAsync_NotExistingKey_ThrowsNotFoundException()
    {
        const string gameKey = "not-existing-game-key";

        _gameRepoMock.Setup(repository => repository.AnyAsync(
                                It.Is<GameByKeySpec>(spec => spec.Key == gameKey)))
                     .ReturnsAsync(false);

        var operation = async () => await _commentService.GetCommentsByGameKeyAsync(gameKey);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }

    [Fact]
    public async void CommentGameAsync_ExistingKey()
    {
        var createModel = new CommentCreateModel { GameKey = "game-key" };

        _gameRepoMock.Setup(repository =>
                                repository.GetSingleOrDefaultBySpecAsync(
                                    It.Is<GameByKeySpec>(spec => spec.Key == createModel.GameKey)))
                     .ReturnsAsync(new Game { Id = Guid.NewGuid() });
        _mapperMock.Setup(mapper => mapper.Map<Comment>(It.IsAny<CommentCreateModel>()))
                   .Returns(new Comment());

        await _commentService.CommentGameAsync(createModel);

        _commentRepoMock.Verify(repository => repository.AddAsync(It.IsAny<Comment>()), Times.Once());
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void CommentGameAsync_NotExistingKey_ThrowsNotFoundException()
    {
        var createModel = new CommentCreateModel { GameKey = "key" };
        Game gameByKey = null!;

        _gameRepoMock.Setup(repository =>
                                repository.GetSingleOrDefaultBySpecAsync(
                                    It.Is<GameByKeySpec>(spec => spec.Key == createModel.GameKey)))
                     .ReturnsAsync(gameByKey);

        var operation = async () => await _commentService.CommentGameAsync(createModel);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }

    [Fact]
    public async void ReplyCommentAsync_CorrectValues()
    {
        var createModel = new ReplyCreateModel { GameKey = "game-key", ParentId = Guid.NewGuid() };
        var expectedGameId = Guid.NewGuid();

        _gameRepoMock
            .Setup(repository =>
                       repository.GetSingleOrDefaultBySpecAsync(
                           It.Is<GameByKeySpec>(spec => spec.Key == createModel.GameKey)))
            .ReturnsAsync(new Game { Id = expectedGameId });
        _commentRepoMock.Setup(repository =>
                                   repository.AnyAsync(
                                       It.Is<CommentByIdSpec>(spec => spec.Id == createModel.ParentId)))
                        .ReturnsAsync(true);
        _mapperMock.Setup(mapper => mapper.Map<Comment>(It.IsAny<ReplyCreateModel>()))
                   .Returns(new Comment());

        await _commentService.ReplyCommentAsync(createModel);

        _commentRepoMock.Verify(repository => repository.AddAsync(It.IsAny<Comment>()), Times.Once);
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void ReplyCommentAsync_NotExistingParentComment_ThrowsNotFoundException()
    {
        var createModel = new ReplyCreateModel { ParentId = Guid.Empty };

        _commentRepoMock.Setup(repository =>
                                   repository.AnyAsync(
                                       It.Is<CommentByIdSpec>(spec => spec.Id == createModel.ParentId)))
                        .ReturnsAsync(false);

        var operation = async () => await _commentService.ReplyCommentAsync(createModel);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }

    [Fact]
    public async void ReplyCommentAsync_NotExistingGame_ThrowsNotFoundException()
    {
        var createModel = new ReplyCreateModel { GameKey = "not-existing-game", ParentId = Guid.NewGuid() };

        _commentRepoMock.Setup(repository =>
                                   repository.AnyAsync(
                                       It.Is<CommentByIdSpec>(spec => spec.Id == createModel.ParentId)))
                        .ReturnsAsync(true);
        _gameRepoMock.Setup(repository =>
                                repository.AnyAsync(It.Is<GameByKeySpec>(spec => spec.Key == createModel.GameKey)))
                     .ReturnsAsync(false);

        var operation = async () => await _commentService.ReplyCommentAsync(createModel);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }

    [Fact]
    public async void UpdateAsync_CorrectValue()
    {
        var commentId = Guid.NewGuid();
        var commentUpdateModel = new CommentUpdateModel
            { Id = commentId, AuthorName = "New author name", Body = "New author body" };


        var comment = new Comment { Id = commentId, Name = "Old author name", Body = "Old author body" };
        _commentRepoMock.Setup(repository =>
                                   repository.GetSingleOrDefaultBySpecAsync(
                                       It.Is<CommentByIdSpec>(spec => spec.Id == commentId)))
                        .ReturnsAsync(comment);

        await _commentService.UpdateAsync(commentUpdateModel);

        _commentRepoMock.Verify(repository => repository.UpdateAsync(comment));
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync());
    }

    [Fact]
    public async void DeleteAsync_CorrectValues_CommentMarkedAsDeleted()
    {
        var commentId = Guid.NewGuid();

        var comment = new Comment { Id = commentId };
        _commentRepoMock.Setup(repository =>
                                   repository.GetSingleOrDefaultBySpecAsync(
                                       It.Is<CommentByIdSpec>(spec => spec.Id == commentId)))
                        .ReturnsAsync(comment);

        await _commentService.DeleteAsync(commentId);

        _commentRepoMock.Verify(repository => repository.UpdateAsync(comment));
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync());
    }
}