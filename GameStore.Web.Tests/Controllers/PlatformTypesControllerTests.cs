using System;
using System.Collections.Generic;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.PlatformTypes;
using GameStore.Core.Models.ServiceModels.PlatformTypes;
using GameStore.Web.Controllers;
using GameStore.Web.Models.PlatformType;
using GameStore.Web.ViewModels.PlatformTypes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers;

public class PlatformTypesControllerTests
{
    private readonly PlatformTypesController _platformController;
    private readonly Mock<IPlatformTypeService> _platformServiceMock;
    private readonly Mock<IMapper> _mapperMock;

    public PlatformTypesControllerTests()
    {
        _platformServiceMock = new Mock<IPlatformTypeService>();
        _mapperMock = new Mock<IMapper>();

        _platformController = new PlatformTypesController(_platformServiceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async void GetAllAsync_NoParameters_ReturnsCorrectValue()
    {
        const int expectedPlatformsCount = 5;

        _platformServiceMock.Setup(service => service.GetAllAsync())
                            .ReturnsAsync(new List<PlatformType>(new PlatformType[expectedPlatformsCount]));
        _mapperMock.Setup(mapper =>
                              mapper.Map<IEnumerable<PlatformTypeListViewModel>>(It.IsAny<ICollection<PlatformType>>()))
                   .Returns(new List<PlatformTypeListViewModel>(new PlatformTypeListViewModel[5]));

        var actualResult = await _platformController.GetAllAsync();
        Assert.IsType<ActionResult<IEnumerable<PlatformTypeListViewModel>>>(actualResult);
    }

    [Fact]
    public async void GetWithDetailsAsync_ExistingPlatform_ReturnsCorrectView()
    {
        var expectedId = Guid.NewGuid();

        _platformServiceMock.Setup(service => service.GetByIdAsync(expectedId))
                            .ReturnsAsync(new PlatformType { Id = expectedId });
        _mapperMock.Setup(mapper => mapper.Map<PlatformTypeViewModel>(It.IsAny<PlatformType>()))
                   .Returns(new PlatformTypeViewModel { Id = expectedId });

        var actualResult = await _platformController.GetWithDetailsAsync(expectedId);

        var actualViewResult = Assert.IsType<ViewResult>(actualResult.Result);
        var actualResultModel = Assert.IsType<PlatformTypeViewModel>(actualViewResult.Model);
        Assert.Equal(expectedId, actualResultModel.Id);
    }

    [Fact]
    public async void GetWithDetails_ExistingPlatform_ReturnsPlatformView()
    {
        var expectedId = Guid.NewGuid();

        _platformServiceMock.Setup(service => service.GetByIdAsync(expectedId))
                            .ReturnsAsync(new PlatformType());
        _mapperMock.Setup(mapper => mapper.Map<PlatformTypeViewModel>(It.IsAny<PlatformType>()))
                   .Returns(new PlatformTypeViewModel { Id = expectedId });

        var actualResult = await _platformController.GetWithDetailsAsync(expectedId);
        Assert.IsType<ActionResult<PlatformTypeViewModel>>(actualResult);
    }

    [Fact]
    public async void CreateAsync_CorrectParameters_ReturnsRedirect()
    {
        _platformServiceMock.Setup(service => service.CreateAsync(It.IsAny<PlatformTypeCreateModel>()))
                            .Verifiable();
        _mapperMock.Setup(mapper => mapper.Map<PlatformTypeCreateModel>(It.IsAny<PlatformTypeCreateRequestModel>()))
                   .Returns(new PlatformTypeCreateModel());

        var actualResult = await _platformController.CreateAsync(new PlatformTypeCreateRequestModel());
        Assert.IsType<RedirectToActionResult>(actualResult);
        _platformServiceMock.Verify(service => service.CreateAsync(It.IsAny<PlatformTypeCreateModel>()), Times.Once());
    }

    [Fact]
    public async void DeleteAsync_ExistingPlatform_ReturnsRedirect()
    {
        var expectedId = Guid.NewGuid();

        _platformServiceMock.Setup(service => service.DeleteAsync(expectedId))
                            .Verifiable();

        var actualResult = await _platformController.DeleteAsync(expectedId);
        Assert.IsType<RedirectToActionResult>(actualResult);
        _platformServiceMock.Verify(service => service.DeleteAsync(expectedId), Times.Once);
    }
}