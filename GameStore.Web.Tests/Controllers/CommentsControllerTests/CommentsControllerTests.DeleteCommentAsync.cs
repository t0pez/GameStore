using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.CommentsControllerTests;

public partial class CommentsControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void DeleteCommentAsync_CorrectValue_ReturnsRedirect(
        Guid commentId,
        string gameKey,
        [Frozen] Mock<ICommentService> commentServiceMock,
        CommentsController sut)
    {
        var actualResult = await sut.DeleteCommentAsync(commentId, gameKey);

        actualResult.Should().BeAssignableTo<RedirectToActionResult>();

        commentServiceMock.Verify(service => service.DeleteAsync(commentId), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async void DeleteCommentAsync_NotCorrectValue_ThrowsException(
        Guid commentId,
        string gameKey,
        [Frozen] Mock<ICommentService> commentServiceMock,
        CommentsController sut)
    {
        var actualResult = await sut.DeleteCommentAsync(commentId, gameKey);

        actualResult.Should().BeAssignableTo<RedirectToActionResult>();

        commentServiceMock.Verify(service => service.DeleteAsync(commentId), Times.Once);
    }
}