using System;
using System.Collections.Generic;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Publishers;
using GameStore.Core.Models.ServiceModels.Publishers;
using GameStore.Web.Controllers;
using GameStore.Web.Models.Publisher;
using GameStore.Web.ViewModels.Publisher;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers;

public class PublisherControllerTests
{
    private readonly PublisherController _publisherController;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IPublisherService> _publisherServiceMock;

    public PublisherControllerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _publisherServiceMock = new Mock<IPublisherService>();

        _publisherController = new PublisherController(_publisherServiceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async void GetAllAsync_NoParameters_ReturnsCorrectValue()
    {
        const int expectedPublishersCount = 5;

        _publisherServiceMock.Setup(service => service.GetAllAsync())
                             .ReturnsAsync(new List<Publisher>(new Publisher[expectedPublishersCount]));
        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<PublisherListViewModel>>(It.IsAny<ICollection<Publisher>>()))
                   .Returns(new List<PublisherListViewModel>(new PublisherListViewModel[5]));

        var actualResult = await _publisherController.GetAllAsync();
        Assert.IsType<ActionResult<IEnumerable<PublisherListViewModel>>>(actualResult);
    }

    [Fact]
    public async void GetWithDetails_ExistingPublisher_ReturnsCorrectValue()
    {
        const string expectedName = "Name";

        _publisherServiceMock.Setup(service => service.GetByCompanyNameAsync(expectedName))
                             .ReturnsAsync(new Publisher());
        _mapperMock.Setup(mapper => mapper.Map<PublisherViewModel>(It.IsAny<Publisher>()))
                   .Returns(new PublisherViewModel { Name = expectedName });

        var actualResult = await _publisherController.GetWithDetailsAsync(expectedName);
        Assert.IsType<ActionResult<PublisherViewModel>>(actualResult);
    }

    [Fact]
    public async void CreateAsync_CorrectParameters_ReturnsRedirect()
    {
        const string expectedName = "Name";

        _publisherServiceMock.Setup(service => service.CreateAsync(It.IsAny<PublisherCreateModel>()))
                             .ReturnsAsync(new Publisher { Name = expectedName })
                             .Verifiable();
        _mapperMock.Setup(mapper => mapper.Map<PublisherCreateModel>(It.IsAny<PublisherCreateRequestModel>()))
                   .Returns(new PublisherCreateModel());

        var actualResult = await _publisherController.CreateAsync(new PublisherCreateRequestModel());
        Assert.IsType<RedirectToActionResult>(actualResult);
        _publisherServiceMock.Verify(service => service.CreateAsync(It.IsAny<PublisherCreateModel>()), Times.Once());
    }

    [Fact]
    public async void DeleteAsync_ExistingPublisher_ReturnsRedirect()
    {
        var expectedId = Guid.NewGuid();

        _publisherServiceMock.Setup(service => service.DeleteAsync(expectedId))
                             .Verifiable();

        var actualResult = await _publisherController.DeleteAsync(expectedId);
        Assert.IsType<RedirectToActionResult>(actualResult);
        _publisherServiceMock.Verify(service => service.DeleteAsync(expectedId), Times.Once);
    }
}