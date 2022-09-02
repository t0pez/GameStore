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
    public async void CreateAsync_NoParameters_ReturnsView(
        List<Genre> genres,
        [Frozen] Mock<IGenreService> genreServiceMock,
        GenresController sut)
    {
        genreServiceMock.Setup(service => service.GetAllAsync())
                        .ReturnsAsync(genres);

        var actualResult = await sut.CreateAsync();

        actualResult.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<GenreCreateRequestModel>();
    }

    [Theory]
    [AutoMoqData]
    public async void CreateAsync_CorrectParameters_ReturnsRedirect(
        GenreCreateRequestModel requestModel,
        [Frozen] Mock<IGenreService> genreServiceMock,
        GenresController sut)
    {
        var actualResult = await sut.CreateAsync(requestModel);

        actualResult.Should().BeOfType<RedirectToActionResult>();

        genreServiceMock.Verify(service => service.CreateAsync(It.IsAny<GenreCreateModel>()), Times.Once());
    }
}