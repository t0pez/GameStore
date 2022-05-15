using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Genres;
using GameStore.Core.Models.ServiceModels.Genres;
using GameStore.Web.Controllers;
using GameStore.Web.Models.Genre;
using GameStore.Web.ViewModels.Genres;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers;

public class GenreControllerTests
{
    private readonly GenresController _genresController;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IGenreService> _genreServiceMock;

    public GenreControllerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _genreServiceMock = new Mock<IGenreService>();

        _genresController = new GenresController(_genreServiceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async void GetAllAsync_NoParameters_ReturnsCorrectValue()
    {
        const int expectedGenresCount = 5;

        _genreServiceMock.Setup(service => service.GetAllAsync())
                             .ReturnsAsync(new List<Genre>(new Genre[expectedGenresCount]));
        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<GenreListViewModel>>(It.IsAny<ICollection<Genre>>()))
                   .Returns(new List<GenreListViewModel>(new GenreListViewModel[5]));

        var actualResult = await _genresController.GetAllAsync();
        Assert.IsType<ActionResult<IEnumerable<GenreListViewModel>>>(actualResult);
    }
    
    [Fact]
    public async void GetWithDetailsAsync_ExistingGenre_ReturnsGenreView()
    {
        var expectedId = Guid.NewGuid();

        _genreServiceMock.Setup(service => service.GetByIdAsync(expectedId))
                         .ReturnsAsync(new Genre { Id = expectedId });
        _mapperMock.Setup(mapper => mapper.Map<GenreViewModel>(It.IsAny<Genre>()))
                   .Returns(new GenreViewModel { Id = expectedId });

        var actualResult = await _genresController.GetWithDetailsAsync(expectedId);

        var actualViewResult = Assert.IsType<ViewResult>(actualResult.Result);
        var actualResultModel = Assert.IsType<GenreViewModel>(actualViewResult.Model);
        Assert.Equal(expectedId, actualResultModel.Id);
    }
    
    [Fact]
    public async void CreateAsync_CorrectParameters_ReturnsRedirect()
    {
        _genreServiceMock.Setup(service => service.CreateAsync(It.IsAny<GenreCreateModel>()))
                         .Verifiable();
        _mapperMock.Setup(mapper => mapper.Map<GenreCreateModel>(It.IsAny<GenreCreateRequestModel>()))
                   .Returns(new GenreCreateModel());

        var actualResult = await _genresController.CreateAsync(new GenreCreateRequestModel());
        Assert.IsType<RedirectToActionResult>(actualResult);
        _genreServiceMock.Verify(service => service.CreateAsync(It.IsAny<GenreCreateModel>()), Times.Once());
    }

    [Fact]
    public async void UpdateAsync_CorrectValues_ReturnsView()
    {
        var currentGenreId = Guid.NewGuid();
        var allGenres = new List<Genre>()
        {
            new()
            {
                Id = currentGenreId
            },
            new()
            {
                Id = Guid.NewGuid()
            }
        };

        _genreServiceMock.Setup(service => service.GetAllAsync())
                         .ReturnsAsync(allGenres);

        var actualResult = await _genresController.UpdateAsync(currentGenreId);

        actualResult.Should().BeAssignableTo<ActionResult<GenreUpdateRequestModel>>();
    }

    [Fact]
    public async void UpdateAsync_CorrectValues_ReturnsRedirect()
    {
        _mapperMock.Setup(mapper => mapper.Map<GenreUpdateModel>(It.IsAny<GenreUpdateRequestModel>()))
                   .Returns(new GenreUpdateModel());
        _genreServiceMock.Setup(service => service.UpdateAsync(It.IsAny<GenreUpdateModel>()))
                         .Verifiable();

        var actualResult = await _genresController.UpdateAsync(new GenreUpdateRequestModel());

        actualResult.Should().BeOfType<RedirectToActionResult>();
        _genreServiceMock.Verify(service => service.UpdateAsync(It.IsAny<GenreUpdateModel>()), Times.Once);
    }

    [Fact]
    public async void DeleteAsync_ExistingGenre_ReturnsRedirect()
    {
        var expectedId = Guid.NewGuid();
        
        _genreServiceMock.Setup(service => service.DeleteAsync(expectedId))
                             .Verifiable();

        var actualResult = await _genresController.DeleteAsync(expectedId);
        Assert.IsType<RedirectToActionResult>(actualResult);
        _genreServiceMock.Verify(service => service.DeleteAsync(expectedId), Times.Once);
    }
}