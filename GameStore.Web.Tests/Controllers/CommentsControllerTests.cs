using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.ServiceModels.Comments;
using GameStore.Web.Controllers;
using GameStore.Web.Models.Comment;
using GameStore.Web.ViewModels.Comments;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers;

public class CommentsControllerTests
{
    private readonly CommentsController _commentsController;
    private readonly Mock<ICommentService> _commentServiceMock;
    private readonly Mock<IMapper> _mapperMock;

    public CommentsControllerTests()
    {
        _commentServiceMock = new Mock<ICommentService>();
        _mapperMock = new Mock<IMapper>();

        _commentsController = new CommentsController(_commentServiceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async void GetCommentsAsync_ExistingGameKey_ReturnsCommentsView()
    {
        const int expectedCommentsCount = 5;

        _commentServiceMock.Setup(service => service.GetCommentsByGameKeyAsync(It.IsAny<string>()))
                           .ReturnsAsync(new List<Comment>(new Comment[expectedCommentsCount]));

        var actualResult = await _commentsController.GetCommentsAsync("");
        Assert.IsType<ActionResult<ICollection<CommentViewModel>>>(actualResult);
    }

    [Fact]
    public async void CommentGameAsync_CorrectValue_ReturnsRedirect()
    {
        var createModel = new CommentCreateRequestModel();

        _mapperMock.Setup(mapper => mapper.Map<CommentCreateModel>(It.IsAny<CommentCreateRequestModel>()))
                   .Returns(new CommentCreateModel());

        var actualResult = await _commentsController.CreateCommentAsync(createModel);

        Assert.IsType<RedirectToActionResult>(actualResult);
        _commentServiceMock.Verify(service => service.CommentGameAsync(It.IsAny<CommentCreateModel>()), Times.Once);
    }

    [Fact]
    public async void UpdateCommentAsync_CorrectValue_ReturnsRedirect()
    {
        var updateRequest = new CommentUpdateRequestModel();
        var updateModel = new CommentUpdateModel();

        _mapperMock.Setup(mapper => mapper.Map<CommentUpdateModel>(updateRequest))
                   .Returns(updateModel);

        var actualResult = await _commentsController.UpdateCommentAsync(updateRequest);

        actualResult.Should().BeAssignableTo<RedirectToActionResult>();
        _commentServiceMock.Verify(service => service.UpdateAsync(updateModel), Times.Once);
    }

    [Fact]
    public async void DeleteCommentAsync_CorrectValue_ReturnsRedirect()
    {
        var commentId = Guid.NewGuid();
        const string gameKey = "game-key";

        var actualResult = await _commentsController.DeleteCommentAsync(commentId, gameKey);

        actualResult.Should().BeAssignableTo<RedirectToActionResult>();
        _commentServiceMock.Verify(service => service.DeleteAsync(commentId), Times.Once);
    }

    [Fact]
    public async void DeleteCommentAsync_NotCorrectValue_ThrowsException()
    {
        var commentId = Guid.NewGuid();
        const string gameKey = "game-key";

        var actualResult = await _commentsController.DeleteCommentAsync(commentId, gameKey);

        actualResult.Should().BeAssignableTo<RedirectToActionResult>();
        _commentServiceMock.Verify(service => service.DeleteAsync(commentId), Times.Once);
    }
}