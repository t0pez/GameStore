using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.GenreControllerTests;

public partial class GenreControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void DeleteAsync_ExistingGenre_ReturnsRedirect(
        Guid genreId,
        [Frozen] Mock<IGenreService> genreServiceMock,
        GenresController sut)
    {
        var actualResult = await sut.DeleteAsync(genreId);

        actualResult.Should().BeOfType<RedirectToActionResult>();

        genreServiceMock.Verify(service => service.DeleteAsync(genreId), Times.Once);
    }
}