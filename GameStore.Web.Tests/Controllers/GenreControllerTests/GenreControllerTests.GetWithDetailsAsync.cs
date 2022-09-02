using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Server.Genres;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using GameStore.Web.ViewModels.Genres;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.GenreControllerTests;

public partial class GenreControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void GetWithDetailsAsync_ExistingGenre_ReturnsGenreView(
        Genre genre,
        [Frozen] Mock<IGenreService> genreServiceMock,
        GenresController sut)
    {
        var expectedId = Guid.NewGuid();

        genreServiceMock.Setup(service => service.GetByIdAsync(expectedId))
                        .ReturnsAsync(genre);

        var actualResult = await sut.GetWithDetailsAsync(expectedId);

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.As<GenreViewModel>().Name.Should().Be(genre.Name);
    }
}