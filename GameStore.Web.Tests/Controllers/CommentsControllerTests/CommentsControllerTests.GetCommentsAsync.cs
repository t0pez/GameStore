using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Server.Comments;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using GameStore.Web.ViewModels.Comments;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.CommentsControllerTests;

public partial class CommentsControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void GetCommentsAsync_ExistingGameKey_ReturnsCommentsView(
        string gameKey,
        List<Comment> comments,
        [Frozen] Mock<ICommentService> commentServiceMock,
        CommentsController sut)
    {
        commentServiceMock.Setup(service => service.GetCommentsByGameKeyAsync(gameKey))
                          .ReturnsAsync(comments);

        var actualResult = await sut.GetCommentsAsync(gameKey);

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeAssignableTo<ICollection<CommentViewModel>>();
    }
}