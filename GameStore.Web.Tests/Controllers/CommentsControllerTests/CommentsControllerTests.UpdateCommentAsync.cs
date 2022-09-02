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
    public async void UpdateCommentAsync_CorrectValue_ReturnsRedirect(
        CommentUpdateRequestModel requestModel,
        [Frozen] Mock<ICommentService> commentServiceMock,
        CommentsController sut)
    {
        var actualResult = await sut.UpdateCommentAsync(requestModel);

        actualResult.Should().BeAssignableTo<RedirectToActionResult>();

        commentServiceMock.Verify(service => service.UpdateAsync(It.IsAny<CommentUpdateModel>()), Times.Once);
    }
}