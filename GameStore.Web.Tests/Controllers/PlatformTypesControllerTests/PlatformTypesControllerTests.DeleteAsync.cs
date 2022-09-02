using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.PlatformTypesControllerTests;

public partial class PlatformTypesControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void DeleteAsync_ExistingPlatform_ReturnsRedirect(
        Guid platformId,
        [Frozen] Mock<IPlatformTypeService> platformServiceMock,
        PlatformTypesController sut)
    {
        var actualResult = await sut.DeleteAsync(platformId);

        actualResult.Should().BeOfType<RedirectToActionResult>();

        platformServiceMock.Verify(service => service.DeleteAsync(platformId), Times.Once);
    }
}