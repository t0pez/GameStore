using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
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
    public async void CreateAsync_NoParameters_ReturnsView()
    {
        var actualResult = await _publisherController.CreateAsync();

        actualResult.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<PublisherCreateRequestModel>();
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
    public async void UpdateAsync_NoParameters_ReturnsView()
    {
        const string companyName = "publisher";
        var publisher = new Publisher
        {
            Name = companyName
        };

        _publisherServiceMock.Setup(service => service.GetByCompanyNameAsync(companyName))
                             .ReturnsAsync(publisher);
        _mapperMock
            .Setup(mapper => mapper.Map<PublisherUpdateRequestModel>(It.Is<Publisher>(p => p.Name == companyName)))
            .Returns(new PublisherUpdateRequestModel { Name = companyName });
        
        var actualResult = await _publisherController.UpdateAsync(companyName);

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<PublisherUpdateRequestModel>()
                    .Which.Name.Should().Be(companyName);
    }

    [Fact]
    public async void UpdateAsync_CorrectParameters_ReturnsRedirect()
    {
        const string expectedName = "Name";
        var request = new PublisherUpdateRequestModel
        {
            Name = expectedName
        };

        _publisherServiceMock
            .Setup(service => service.UpdateAsync(It.Is<PublisherUpdateModel>(model => model.Name == expectedName)))
            .Verifiable();
        _mapperMock.Setup(mapper => mapper.Map<PublisherUpdateModel>(request))
                   .Returns(new PublisherUpdateModel { Name = expectedName });

        var actualResult = await _publisherController.UpdateAsync(request);

        actualResult.Should().BeOfType<RedirectToActionResult>();
        _publisherServiceMock.Verify(service => service.UpdateAsync(It.Is<PublisherUpdateModel>(model => model.Name == expectedName)), Times.Once());
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