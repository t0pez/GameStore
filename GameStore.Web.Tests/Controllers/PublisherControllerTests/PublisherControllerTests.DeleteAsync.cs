using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.PublisherControllerTests;

public partial class PublisherControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void DeleteAsync_ExistingPublisher_ReturnsRedirect(
        string publisherName,
        [Frozen] Mock<IPublisherService> publisherServiceMock,
        PublisherController sut)
    {
        var actualResult = await sut.DeleteAsync(publisherName);

        actualResult.Should().BeOfType<RedirectToActionResult>();

        publisherServiceMock.Verify(service => service.DeleteAsync(publisherName), Times.Once);
    }
}