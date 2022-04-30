﻿using System;
using System.Collections.Generic;
using AutoMapper;
using GameStore.Core.Models.Genres;
using GameStore.Core.Models.Genres.Specifications;
using GameStore.Core.Models.ServiceModels.Genres;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services;

public class GenreServiceTests
{
    private readonly GenreService _genreService;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Genre>> _genreRepoMock;

    public GenreServiceTests()
    {
        _mapperMock = new Mock<IMapper>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _genreRepoMock = new Mock<IRepository<Genre>>();

        _unitOfWorkMock.Setup(unit => unit.GetRepository<Genre>())
                       .Returns(_genreRepoMock.Object);

        _genreService = new GenreService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async void GetAllAsync_ReturnsCorrectValues()
    {
        const int expectedCount = 4;

        _genreRepoMock.Setup(repository => repository.GetBySpecAsync(It.IsAny<GenresListSpec>()))
                      .ReturnsAsync(new List<Genre>(new Genre[expectedCount]));

        var actualResult = await _genreService.GetAllAsync();
        var actualCount = actualResult.Count;

        Assert.Equal(expectedCount, actualCount);
    }

    [Fact]
    public async void GetWithDetailsAsync_ReturnsCorrectValues()
    {
        var expectedId = Guid.NewGuid();

        _genreRepoMock.Setup(repository => repository.GetSingleOrDefaultBySpecAsync(
                                 It.Is<GenreByIdWithDetailsSpec>(spec => spec.Id == expectedId)))
                      .ReturnsAsync(new Genre { Id = expectedId });

        var actualResult = await _genreService.GetByIdAsync(expectedId);

        Assert.Equal(expectedId, actualResult.Id);
    }

    [Fact]
    public async void CreateAsync_CorrectValues_NoReturnValue()
    {
        var createModel = new GenreCreateModel();

        _mapperMock.Setup(mapper => mapper.Map<Genre>(It.IsAny<GenreCreateModel>()))
                   .Returns(new Genre());
        _genreRepoMock.Setup(repository => repository.AddAsync(It.IsAny<Genre>()))
                      .Verifiable();

        await _genreService.CreateAsync(createModel);

        _genreRepoMock.Verify(repository => repository.AddAsync(It.IsAny<Genre>()), Times.Once);
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void UpdateAsync_CorrectValues_NoReturnValue()
    {
        const string expectedName = "New name";
        var expectedId = Guid.NewGuid();
        var actualGenre = new Genre { Id = expectedId, Name = "Old name" };
        var updatedGenre = new Genre { Id = expectedId, Name = "New name" };
        var updateModel = new GenreUpdateModel { Id = expectedId, Name = expectedName };

        _genreRepoMock.Setup(repository => repository.GetSingleOrDefaultBySpecAsync(
                                 It.Is<GenreByIdSpec>(spec => spec.Id == expectedId)))
                      .ReturnsAsync(actualGenre);
        _genreRepoMock.Setup(repository => repository.UpdateAsync(
                                 It.Is<Genre>(model => model.Name == expectedName)))
                      .Verifiable();

        await _genreService.UpdateAsync(updateModel);

        Assert.Equal(updatedGenre.Id, actualGenre.Id);
        Assert.Equal(updatedGenre.Name, actualGenre.Name);
        _genreRepoMock.Verify(
            repository =>
                repository.UpdateAsync(It.Is<Genre>(type => type.Id == expectedId && type.Name == expectedName)),
            Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void DeleteAsync_CorrectValues_GenreSoftDeleted()
    {
        _genreRepoMock.Setup(repository => repository.GetSingleOrDefaultBySpecAsync(It.IsAny<GenreByIdSpec>()))
                      .ReturnsAsync(new Genre());

        await _genreService.DeleteAsync(Guid.NewGuid());

        _genreRepoMock.Verify(repository => repository.UpdateAsync(It.IsAny<Genre>()), Times.Once);
        _unitOfWorkMock.Verify(work => work.SaveChangesAsync(), Times.Once);
    }
}