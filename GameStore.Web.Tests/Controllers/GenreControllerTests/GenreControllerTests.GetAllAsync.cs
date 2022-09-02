using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Server.Genres;
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
    public async void GetAllAsync_NoParameters_ReturnsCorrectValue(
        List<Genre> genres,
        [Frozen] Mock<IGenreService> genreServiceMock,
        GenresController sut
    )
    {
        genreServiceMock.Setup(service => service.GetAllAsync())
                        .ReturnsAsync(genres);

        var actualResult = await sut.GetAllAsync();

        actualResult.Result.Should().BeOfType<ViewResult>();
    }
}