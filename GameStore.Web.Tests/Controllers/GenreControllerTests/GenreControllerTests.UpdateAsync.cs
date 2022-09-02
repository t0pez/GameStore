using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Server.Genres;
using GameStore.Core.Models.ServiceModels.Genres;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using GameStore.Web.Models.Genre;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.GenreControllerTests;

public partial class GenreControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void UpdateAsync_CorrectValues_ReturnsView(
        Guid currentGenreId,
        List<Genre> genres,
        [Frozen] Mock<IGenreService> genreServiceMock,
        GenresController sut)
    {
        genreServiceMock.Setup(service => service.GetAllAsync())
                        .ReturnsAsync(genres);

        var actualResult = await sut.UpdateAsync(currentGenreId);

        actualResult.Should().BeAssignableTo<ActionResult<GenreUpdateRequestModel>>();
    }

    [Theory]
    [AutoMoqData]
    public async void UpdateAsync_IncorrectValues_ReturnsBadResult(
        List<Genre> genres,
        [Frozen] Mock<IGenreService> genreServiceMock,
        GenresController sut)
    {
        genreServiceMock.Setup(service => service.GetAllAsync())
                        .ReturnsAsync(genres);

        var actualResult = await sut.UpdateAsync(Guid.Empty);

        actualResult.Result.Should().BeAssignableTo<BadRequestResult>();
    }

    [Theory]
    [AutoMoqData]
    public async void UpdateAsync_CorrectValues_ReturnsRedirect(
        [Frozen] Mock<IGenreService> genreServiceMock,
        GenresController sut)
    {
        var actualResult = await sut.UpdateAsync(new GenreUpdateRequestModel());

        actualResult.Should().BeOfType<RedirectToActionResult>();
        genreServiceMock.Verify(service => service.UpdateAsync(It.IsAny<GenreUpdateModel>()), Times.Once);
    }
}