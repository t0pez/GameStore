using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.Loggers;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Mongo.Suppliers;
using GameStore.Core.Models.Publishers;
using GameStore.Core.Models.Publishers.Specifications;
using GameStore.Core.Models.ServiceModels.Publishers;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MediatR;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services;

public class PublisherServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepository<Publisher>> _publisherRepoMock;
    private readonly PublisherService _publisherService;
    private readonly Mock<ISearchService> _searchServiceMock;
    private readonly Mock<IRepository<Supplier>> _supplierRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public PublisherServiceTests()
    {
        var mongoLoggerMock = new Mock<IMongoLogger>();
        var mediatorMock = new Mock<IMediator>();
        _searchServiceMock = new Mock<ISearchService>();
        _mapperMock = new Mock<IMapper>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _publisherRepoMock = new Mock<IRepository<Publisher>>();
        _supplierRepoMock = new Mock<IRepository<Supplier>>();

        _unitOfWorkMock.Setup(unit => unit.GetEfRepository<Publisher>())
                       .Returns(_publisherRepoMock.Object);
        _unitOfWorkMock.Setup(unit => unit.GetMongoRepository<Supplier>())
                       .Returns(_supplierRepoMock.Object);

        _publisherService = new PublisherService(_searchServiceMock.Object, mediatorMock.Object,
                                                 mongoLoggerMock.Object, _unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async void GetAllAsync_ReturnsCorrectValues()
    {
        const int expectedCount = 4;

        var publishers = new List<Publisher>
        {
            new() { Name = "1" },
            new() { Name = "2" }
        };
        var suppliers = new List<Supplier>
        {
            new() { CompanyName = "3" },
            new() { CompanyName = "4" }
        };
        var mappedPublishers = new List<PublisherDto>
        {
            new() { Name = "1" },
            new() { Name = "2" }
        };
        var mappedSuppliers = new List<PublisherDto>
        {
            new() { Name = "3" },
            new() { Name = "4" }
        };

        _publisherRepoMock.Setup(repository => repository.GetBySpecAsync(It.IsAny<PublishersListSpec>()))
                          .ReturnsAsync(publishers);
        _supplierRepoMock.Setup(repository => repository.GetBySpecAsync(null))
                         .ReturnsAsync(suppliers);
        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<PublisherDto>>(publishers))
                   .Returns(mappedPublishers);
        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<PublisherDto>>(suppliers))
                   .Returns(mappedSuppliers);

        var actualResult = await _publisherService.GetAllAsync();
        var actualCount = actualResult.Count;

        expectedCount.Should().Be(actualCount);
    }

    [Fact]
    public async void GetWithDetailsAsync_ReturnsCorrectValues()
    {
        const string expectedName = "Name";

        var publisher = new PublisherDto
        {
            Name = expectedName
        };

        _searchServiceMock.Setup(service => service.GetPublisherDtoByCompanyNameOrDefaultAsync(expectedName))
                          .ReturnsAsync(publisher);

        var actualResult = await _publisherService.GetByCompanyNameAsync(expectedName);

        expectedName.Should().Be(actualResult.Name);
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
        const string oldName = "Old name";
        const string expectedName = "New name";

        var publisher = new Publisher { Name = oldName };
        var updatedPublisher = new Publisher { Name = expectedName };
        var updateModel = new PublisherUpdateModel { OldName = oldName, Name = expectedName };

        _publisherRepoMock.Setup(repository => repository.GetSingleOrDefaultBySpecAsync(
                                     It.Is<PublisherByNameSpec>(spec => spec.Name == oldName)))
                          .ReturnsAsync(publisher);
        _publisherRepoMock.Setup(repository => repository.UpdateAsync(
                                     It.Is<Publisher>(model => model.Name == expectedName)))
                          .Verifiable();

        await _publisherService.UpdateAsync(updateModel);

        updatedPublisher.Name.Should().Be(publisher.Name);

        _publisherRepoMock.Verify(
            repository =>
                repository.UpdateAsync(It.Is<Publisher>(pub => pub.Name == expectedName)),
            Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void DeleteAsync_CorrectValues_PlatformSoftDeleted()
    {
        const string expectedName = "Company name";

        _publisherRepoMock.Setup(repository =>
                                     repository.GetSingleOrDefaultBySpecAsync(
                                         It.Is<PublisherByNameSpec>(spec => spec.Name == expectedName)))
                          .ReturnsAsync(new Publisher());

        await _publisherService.DeleteAsync(expectedName);

        _publisherRepoMock.Verify(
            repository => repository.UpdateAsync(It.Is<Publisher>(publisher => publisher.IsDeleted == true)),
            Times.Once);
        _unitOfWorkMock.Verify(work => work.SaveChangesAsync(), Times.Once);
    }
}