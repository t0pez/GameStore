using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Dto;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using GameStore.Web.Interfaces;
using GameStore.Web.ViewModels.Publisher;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.PublisherControllerTests;

public partial class PublisherControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void GetAllAsync_NoParameters_ReturnsCorrectValue(
        List<PublisherDto> publishers,
        [Frozen] Mock<IPublisherService> publisherServiceMock,
        [Frozen] Mock<IPublisherAuthHelper> publisherAuthMock,
        PublisherController sut)
    {
        publisherAuthMock
           .Setup(helper => helper.CanEditAsync(It.IsAny<string>()))
           .ReturnsAsync(true);

        publisherServiceMock.Setup(service => service.GetAllAsync())
                            .ReturnsAsync(publishers);

        var actualResult = await sut.GetAllAsync();

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeAssignableTo<IEnumerable<PublisherListViewModel>>();
    }
}