using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.ServiceModels.Publishers;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using GameStore.Web.Interfaces;
using GameStore.Web.Models.Publisher;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.PublisherControllerTests;

public partial class PublisherControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void UpdateAsync_NoParameters_ReturnsView(
        PublisherDto publisher,
        [Frozen] Mock<IPublisherService> publisherServiceMock,
        [Frozen] Mock<IPublisherAuthHelper> publisherAuthMock,
        PublisherController sut)
    {
        publisherServiceMock.Setup(service => service.GetByCompanyNameAsync(publisher.Name))
                            .ReturnsAsync(publisher);

        publisherAuthMock
           .Setup(helper => helper.CanEditAsync(It.IsAny<string>()))
           .ReturnsAsync(true);

        var actualResult = await sut.UpdateAsync(publisher.Name);

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<PublisherUpdateRequestModel>()
                    .Which.Name.Should().Be(publisher.Name);
    }

    [Theory]
    [AutoMoqData]
    public async void UpdateAsync_CorrectParameters_ReturnsRedirect(
        PublisherUpdateRequestModel requestModel,
        [Frozen] Mock<IPublisherService> publisherServiceMock,
        [Frozen] Mock<IPublisherAuthHelper> publisherAuthMock,
        PublisherController sut)
    {
        publisherAuthMock
           .Setup(helper => helper.CanEditAsync(It.IsAny<string>()))
           .ReturnsAsync(true);

        var actualResult = await sut.UpdateAsync(requestModel, requestModel.Name);

        actualResult.Should().BeOfType<RedirectToActionResult>();

        publisherServiceMock.Verify(
            service => service.UpdateAsync(It.IsAny<PublisherUpdateModel>()),
            Times.Once());
    }
}