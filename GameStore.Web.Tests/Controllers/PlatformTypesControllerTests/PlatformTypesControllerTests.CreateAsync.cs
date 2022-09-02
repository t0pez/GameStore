using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
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
    public async void CreateAsync_NoParameters_ReturnsView(
        PlatformTypesController sut)
    {
        var actualResult = await sut.CreateAsync();

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<PlatformTypeCreateRequestModel>();
    }

    [Theory]
    [AutoMoqData]
    public async void CreateAsync_CorrectParameters_ReturnsRedirect(
        [Frozen] Mock<IPlatformTypeService> platformServiceMock,
        PlatformTypesController sut)
    {
        var actualResult = await sut.CreateAsync(new PlatformTypeCreateRequestModel());

        actualResult.Should().BeOfType<RedirectToActionResult>();

        platformServiceMock.Verify(service => service.CreateAsync(It.IsAny<PlatformTypeCreateModel>()), Times.Once());
    }
}