using System;
using System.Collections.Generic;
using AutoMapper;
using GameStore.Core.Interfaces.Loggers;
using GameStore.Core.Models.PlatformTypes;
using GameStore.Core.Models.PlatformTypes.Specifications;
using GameStore.Core.Models.ServiceModels.PlatformTypes;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services;

public class PlatformTypeServiceTests
{
    private readonly PlatformTypeService _platformService;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<PlatformType>> _platformRepoMock;

    public PlatformTypeServiceTests()
    {
        var mongoLogger = new Mock<IMongoLogger>();
        _mapperMock = new Mock<IMapper>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _platformRepoMock = new Mock<IRepository<PlatformType>>();

        _unitOfWorkMock.Setup(unit => unit.GetEfRepository<PlatformType>())
                       .Returns(_platformRepoMock.Object);

        _platformService = new PlatformTypeService(mongoLogger.Object, _unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async void GetAllAsync_ReturnsCorrectValues()
    {
        const int expectedCount = 4;

        _platformRepoMock.Setup(repository => repository.GetBySpecAsync(It.IsAny<PlatformTypesListSpec>()))
                         .ReturnsAsync(new List<PlatformType>(new PlatformType[expectedCount]));

        var actualResult = await _platformService.GetAllAsync();
        var actualCount = actualResult.Count;

        Assert.Equal(expectedCount, actualCount);
    }

    [Fact]
    public async void GetWithDetailsAsync_ExistingPlatform_ReturnsCorrectValues()
    {
        var expectedId = Guid.NewGuid();

        _platformRepoMock.Setup(repository => repository.GetSingleOrDefaultBySpecAsync(
                                    It.Is<PlatformTypeByIdSpec>(spec => spec.Id == expectedId)))
                         .ReturnsAsync(new PlatformType { Id = expectedId });

        var actualResult = await _platformService.GetByIdAsync(expectedId);

        Assert.Equal(expectedId, actualResult.Id);
    }

    [Fact]
    public async void CreateAsync_CorrectValues_NoReturnValue()
    {
        var createModel = new PlatformTypeCreateModel();

        _mapperMock.Setup(mapper => mapper.Map<PlatformType>(It.IsAny<PlatformTypeCreateModel>()))
                   .Returns(new PlatformType());
        _platformRepoMock.Setup(repository => repository.AddAsync(It.IsAny<PlatformType>()))
                         .Verifiable();


        await _platformService.CreateAsync(createModel);

        _platformRepoMock.Verify(repository => repository.AddAsync(It.IsAny<PlatformType>()), Times.Once);
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void UpdateAsync_CorrectValues_NoReturnValue()
    {
        const string expectedName = "New name";
        var expectedId = Guid.NewGuid();
        var platformType = new PlatformType { Id = expectedId, Name = "Old name" };
        var updatedPlatformType = new PlatformType { Id = expectedId, Name = "New name" };
        var updateModel = new PlatformTypeUpdateModel { Id = expectedId, Name = expectedName };

        _platformRepoMock.Setup(repository => repository.GetSingleOrDefaultBySpecAsync(
                                    It.Is<PlatformTypeByIdSpec>(spec => spec.Id == expectedId)))
                         .ReturnsAsync(platformType);
        _platformRepoMock.Setup(repository => repository.UpdateAsync(
                                    It.Is<PlatformType>(model => model.Name == expectedName)))
                         .Verifiable();

        await _platformService.UpdateAsync(updateModel);

        Assert.Equal(updatedPlatformType.Id, platformType.Id);
        Assert.Equal(updatedPlatformType.Name, platformType.Name);
        _platformRepoMock.Verify(
            repository =>
                repository.UpdateAsync(It.Is<PlatformType>(type => type.Id == expectedId && type.Name == expectedName)),
            Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void DeleteAsync_CorrectValues_PlatformSoftDeleted()
    {
        var expectedId = Guid.NewGuid();

        _platformRepoMock
            .Setup(repository =>
                       repository.GetSingleOrDefaultBySpecAsync(
                           It.Is<PlatformTypeByIdSpec>(spec => spec.Id == expectedId)))
            .ReturnsAsync(new PlatformType());

        await _platformService.DeleteAsync(expectedId);

        _platformRepoMock.Verify(repository => repository.UpdateAsync(It.IsAny<PlatformType>()), Times.Once);
        _unitOfWorkMock.Verify(work => work.SaveChangesAsync(), Times.Once);
    }
}