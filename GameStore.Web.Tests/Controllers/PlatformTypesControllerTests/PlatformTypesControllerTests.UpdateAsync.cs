using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Server.PlatformTypes;
using GameStore.Core.Models.ServiceModels.PlatformTypes;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using GameStore.Web.Models.PlatformType;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.PlatformTypesControllerTests;

public partial class PlatformTypesControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void UpdateAsync_ExistingPlatformType_ReturnsView(
        PlatformType platform,
        [Frozen] Mock<IPlatformTypeService> platformServiceMock,
        PlatformTypesController sut)
    {
        platformServiceMock.Setup(service => service.GetByIdAsync(platform.Id))
                           .ReturnsAsync(platform);

        var actualResult = await sut.UpdateAsync(platform.Id);

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<PlatformTypeUpdateRequestModel>()
                    .And.Subject.As<PlatformTypeUpdateRequestModel>().Id.Should().Be(platform.Id);
    }

    [Theory]
    [AutoMoqData]
    public async void UpdateAsync_CorrectParameters_ReturnsRedirect(
        [Frozen] Mock<IPlatformTypeService> platformServiceMock,
        PlatformTypesController sut)
    {
        var actualResult = await sut.UpdateAsync(new PlatformTypeUpdateRequestModel());

        actualResult.Should().BeOfType<RedirectToActionResult>();

        platformServiceMock.Verify(service => service.UpdateAsync(It.IsAny<PlatformTypeUpdateModel>()), Times.Once());
    }
}