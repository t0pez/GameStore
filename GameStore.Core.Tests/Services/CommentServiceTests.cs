using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Comments.Specifications;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.Core.Models.Records;
using GameStore.Core.Profiles;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services;

public class CommentServiceTests
{
    private readonly ICommentService _commentService;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CommentServiceTests()
    {
        _unitOfWorkMock = GetUnitOfWorkMock();
        var logger = new Mock<ILogger<CommentService>>();
        var mapper = new Mapper(new MapperConfiguration(expression => expression.AddProfile(new CoreCommonProfile())));

        _commentService = new CommentService(_unitOfWorkMock.Object, logger.Object, mapper);
    }
    
    [Fact]
    public async void GetCommentsByGameAsync_ExistingKey_ReturnsComments()
    {
        const int expectedResultCount = 3;

        var actualResult = await _commentService.GetCommentsByGameKeyAsync(AllGames[0].Key);
        var actualResultCount = actualResult.Count;
        
        Assert.Equal(expectedResultCount, actualResultCount);
    }
    
    [Fact]
    public async void GetCommentsByGameAsync_NotExistingKey_ThrowsNotFoundException()
    {
        var operation = async () => await _commentService.GetCommentsByGameKeyAsync("AllGames[0].Key");

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }
    
    [Fact]
    public async void CommentGameAsync_ExistingKey()
    {
        var creationModel = new CommentCreateModel
        {
            GameKey = AllGames[0].Key,
            AuthorName = "Some author",
            Message = "asdasd"
        };

        await _commentService.CommentGameAsync(creationModel);
        
        Assert.True(true);
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async void CommentGameAsync_NotExistingKey_ThrowsNotFoundException()
    {
        var creationModel = new CommentCreateModel
        {
            GameKey = "AllGames[0].Key",
            AuthorName = "Some author",
            Message = "asdasd"
        };
        
        var operation = async () => await _commentService.CommentGameAsync(creationModel);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }
    
    [Fact]
    public async void ReplyCommentAsync_CorrectValues()
    {
        var creationModel = new ReplyCreateModel
        {
            GameId = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601"),
            ParentId = Guid.Parse("6fd6d158-0000-472a-b973-08da067d7601"),
            Message = "Some message",
            AuthorName = "Some author"
        };

        await _commentService.ReplyCommentAsync(creationModel);
        
        Assert.True(true);
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async void ReplyCommentAsync_NotExistingParentComment_ThrowsNotFoundException()
    {
        var creationModel = new ReplyCreateModel
        {
            GameId = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601"),
            ParentId = Guid.Parse("00000000-0010-472a-b973-08da067d7601"),
            Message = "Some message",
            AuthorName = "Some author"
        };

        var operation = async () => await _commentService.ReplyCommentAsync(creationModel);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }
    
    [Fact]
    public async void ReplyCommentAsync_NotExistingGame_ThrowsNotFoundException()
    {
        var creationModel = new ReplyCreateModel
        {
            GameId = Guid.Parse("00000000-7ffd-472a-b971-08da067d7601"),
            ParentId = Guid.Parse("6fd6d158-0000-472a-b973-08da067d7601"),
            Message = "Some message",
            AuthorName = "Some author"
        };

        var operation = async () => await _commentService.ReplyCommentAsync(creationModel);

        await Assert.ThrowsAsync<ItemNotFoundException>(operation);
    }
    
    private static Mock<IUnitOfWork> GetUnitOfWorkMock()
    {
        var mock = new Mock<IUnitOfWork>();
        var gameRepository = GetGameRepositoryMock();
        var commentRepository = GetCommentRepositoryMock();
        
        mock.Setup(unitOfWork => unitOfWork.GetRepository<Game>())
            .Returns(gameRepository.Object);
        mock.Setup(unitOfWork => unitOfWork.GetRepository<Comment>())
            .Returns(commentRepository.Object);
        mock.Setup(unitOfWork => unitOfWork.SaveChangesAsync())
            .ReturnsAsync(0);

        return mock;
    }

    private static Mock<IRepository<Game>> GetGameRepositoryMock()
    {
        var mock = new Mock<IRepository<Game>>();

        mock.Setup(repository => repository.GetSingleBySpecAsync(
                       It.Is<GameByKeySpec>(spec => spec.Key == AllGames[0].Key)))
            .ReturnsAsync(AllGames[0]);
        
        mock.Setup(repository => repository.AnyAsync(
                       It.Is<GameByIdSpec>(spec => spec.Id == AllGames[0].Id)))
            .ReturnsAsync(true);
        
        mock.Setup(repository => repository.AnyAsync(
                       It.Is<GameByKeySpec>(spec => spec.Key == AllGames[0].Key)))
            .ReturnsAsync(true);

        return mock;
    }
    
    private static Mock<IRepository<Comment>> GetCommentRepositoryMock()
    {
        var mock = new Mock<IRepository<Comment>>();
        
        mock.Setup(repository => repository.GetBySpecAsync(
                       It.Is<CommentsByGameKeySpec>(spec => spec.GameKey == AllGames[0].Key)))
            .ReturnsAsync(AllComments.Where(comment => comment.GameId == AllGames[0].Id).ToList);
        
        mock.Setup(repository => repository.GetSingleBySpecAsync(
                       It.Is<CommentByIdSpec>(spec => spec.Id == AllComments[0].Id)))
            .ReturnsAsync(AllComments[0]);
        
        mock.Setup(repository => repository.AnyAsync(
                       It.Is<CommentByIdSpec>(spec => spec.Id == AllComments[0].Id)))
            .ReturnsAsync(true);

        return mock;
    }
    
    private static readonly List<Game> AllGames = new()
    {
        new()
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601"),
            Name = "First game",
            Key = "first-game",
            Description = "First description",
            File = new byte[] { 0, 0, 0, 1 },
            IsDeleted = false
        },
        new()
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601"),
            Name = "Second game",
            Key = "second-game",
            Description = "Second description",
            File = new byte[] { 0, 0, 0, 2 },
            IsDeleted = false
        },
        new()
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b973-08da067d7601"),
            Name = "Third game",
            Key = "Third-game",
            Description = "Third description",
            File = new byte[] { 0, 0, 0, 3 },
            IsDeleted = false
        },
        new()
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601"),
            Name = "Fourth game",
            Key = "fourth-game",
            Description = "Fourth description",
            File = new byte[] { 0, 0, 0, 4 },
            IsDeleted = false
        }
    };

    private static readonly List<Comment> AllComments = new()
    {
        new()
        {
            Id = Guid.Parse("6fd6d158-0000-472a-b973-08da067d7601"),
            Name = "Author 1",
            Body = "Message 1",
            GameId = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601"),
            DateOfCreation = DateTime.UtcNow.AddDays(-1),
            ParentId = null
        },
        new()
        {
            Id = Guid.Parse("6fd6d158-0010-472a-b973-08da067d7601"),
            Name = "Author 2",
            Body = "Message 2",
            GameId = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601"),
            DateOfCreation = DateTime.UtcNow.AddDays(-4),
            ParentId = null
        },
        new()
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601"),
            Name = "Author 3",
            Body = "Message 3",
            GameId = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601"),
            DateOfCreation = DateTime.UtcNow.AddDays(-2),
            ParentId = Guid.Parse("6fd6d158-0010-472a-b973-08da067d7601")
        }
    };
}