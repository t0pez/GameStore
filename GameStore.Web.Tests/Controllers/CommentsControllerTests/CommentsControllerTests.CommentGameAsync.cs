using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.ServiceModels.Comments;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using GameStore.Web.Models.Comment;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.CommentsControllerTests;

public partial class CommentsControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void CommentGameAsync_CorrectValue_ReturnsRedirect(
        CommentCreateRequestModel createModel,
        [Frozen] Mock<ICommentService> commentServiceMock,
        CommentsController sut)
    {
        createModel.ParentId = null;

        var actualResult = await sut.CreateCommentAsync(createModel);

        actualResult.Should().BeOfType<RedirectToActionResult>();

        commentServiceMock.Verify(service => service.CommentGameAsync(It.IsAny<CommentCreateModel>()), Times.Once);
    }
}