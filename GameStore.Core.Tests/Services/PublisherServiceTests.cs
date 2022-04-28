using System;
using System.Collections.Generic;
using AutoMapper;
using GameStore.Core.Models.Publishers;
using GameStore.Core.Models.Publishers.Specifications;
using GameStore.Core.Models.ServiceModels.Publishers;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services;

public class PublisherServiceTests
{
    private readonly PublisherService _publisherService;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Publisher>> _publisherRepoMock;

    public PublisherServiceTests()
    {
        var loggerMock = new Mock<ILogger<PublisherService>>();
        _mapperMock = new Mock<IMapper>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _publisherRepoMock = new Mock<IRepository<Publisher>>();

        _unitOfWorkMock.Setup(unit => unit.GetRepository<Publisher>())
                       .Returns(_publisherRepoMock.Object);

        _publisherService = new PublisherService(_unitOfWorkMock.Object, _mapperMock.Object, loggerMock.Object);
    }

    [Fact]
    public async void GetAllAsync_ReturnsCorrectValues()
    {
        const int expectedCount = 4;

        _publisherRepoMock.Setup(repository => repository.GetBySpecAsync(It.IsAny<PublishersWithDetailsSpec>()))
                          .ReturnsAsync(new List<Publisher>(new Publisher[expectedCount]));

        var actualResult = await _publisherService.GetAllAsync();
        var actualCount = actualResult.Count;

        Assert.Equal(expectedCount, actualCount);
    }

    [Fact]
    public async void GetWithDetailsAsync_ReturnsCorrectValues()
    {
        var expectedId = Guid.NewGuid();
        const string expectedName = "Name";

        _publisherRepoMock.Setup(repository => repository.GetSingleOrDefaultBySpecAsync(
                                     It.Is<PublisherByCompanyNameSpec>(spec => spec.Name == expectedName)))
                          .ReturnsAsync(new Publisher { Id = expectedId, Name = expectedName });

        var actualResult = await _publisherService.GetByCompanyNameAsync(expectedName);

        Assert.Equal(expectedId, actualResult.Id);
    }

    [Fact]
    public async void CreateAsync_CorrectValues_NoReturnValue()
    {
        var createModel = new PublisherCreateModel();

        _mapperMock.Setup(mapper => mapper.Map<Publisher>(It.IsAny<PublisherCreateModel>()))
                   .Returns(new Publisher());
        _publisherRepoMock.Setup(repository => repository.AddAsync(It.IsAny<Publisher>()))
                          .Verifiable();

        await _publisherService.CreateAsync(createModel);

        _publisherRepoMock.Verify(repository => repository.AddAsync(It.IsAny<Publisher>()), Times.Once);
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void UpdateAsync_CorrectValues_NoReturnValue()
    {
        const string expectedName = "New name";
        var expectedId = Guid.NewGuid();
        var publisher = new Publisher { Id = expectedId, Name = "Old name" };
        var updatedPublisher = new Publisher { Id = expectedId, Name = "New name" };
        var updateModel = new PublisherUpdateModel { Id = expectedId, Name = expectedName };

        _publisherRepoMock.Setup(repository => repository.GetSingleOrDefaultBySpecAsync(
                                     It.Is<PublisherByIdWithDetailsSpec>(spec => spec.Id == expectedId)))
                          .ReturnsAsync(publisher);
        _publisherRepoMock.Setup(repository => repository.UpdateAsync(
                                     It.Is<Publisher>(model => model.Name == expectedName)))
                          .Verifiable();

        await _publisherService.UpdateAsync(updateModel);

        Assert.Equal(updatedPublisher.Id, publisher.Id);
        Assert.Equal(updatedPublisher.Name, publisher.Name);
        _publisherRepoMock.Verify(
            repository =>
                repository.UpdateAsync(It.Is<Publisher>(type => type.Id == expectedId && type.Name == expectedName)),
            Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void DeleteAsync_CorrectValues_PlatformSoftDeleted()
    {
        _publisherRepoMock.Setup(repository => repository.GetSingleOrDefaultBySpecAsync(It.IsAny<PublisherByIdSpec>()))
                          .ReturnsAsync(new Publisher());

        await _publisherService.DeleteAsync(Guid.NewGuid());

        _publisherRepoMock.Verify(repository => repository.UpdateAsync(It.IsAny<Publisher>()), Times.Once);
        _unitOfWorkMock.Verify(work => work.SaveChangesAsync(), Times.Once);
    }
}