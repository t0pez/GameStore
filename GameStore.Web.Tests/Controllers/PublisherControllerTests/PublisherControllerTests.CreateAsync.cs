using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Server.Publishers;
using GameStore.Core.Models.ServiceModels.Publishers;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using GameStore.Web.Models.Publisher;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.PublisherControllerTests;

public partial class PublisherControllerTests
{
    [Theory]
    [AutoMoqData]
    public async Task CreateAsync_NoParameters_ReturnsView(
        PublisherController sut)
    {
        var actualResult = await sut.CreateAsync();

        actualResult.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<PublisherCreateRequestModel>();
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateAsync_CorrectParameters_ReturnsRedirect(
        Publisher publisher,
        [Frozen] Mock<IPublisherService> publisherServiceMock,
        PublisherController sut)
    {
        publisherServiceMock.Setup(service => service.CreateAsync(It.IsAny<PublisherCreateModel>()))
                            .ReturnsAsync(publisher);

        var actualResult = await sut.CreateAsync(new PublisherCreateRequestModel());

        actualResult.Should().BeOfType<RedirectToActionResult>();

        publisherServiceMock.Verify(service => service.CreateAsync(It.IsAny<PublisherCreateModel>()), Times.Once());
    }
}