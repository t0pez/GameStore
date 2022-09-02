using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Server.PlatformTypes;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using GameStore.Web.ViewModels.PlatformTypes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers.PlatformTypesControllerTests;

public partial class PlatformTypesControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void GetWithDetails_ExistingPlatform_ReturnsPlatformView(
        PlatformType platform,
        [Frozen] Mock<IPlatformTypeService> platformServiceMock,
        PlatformTypesController sut)
    {
        platformServiceMock.Setup(service => service.GetByIdAsync(platform.Id))
                           .ReturnsAsync(platform);

        var actualResult = await sut.GetWithDetailsAsync(platform.Id);

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.As<PlatformTypeViewModel>().Id.Should().Be(platform.Id);
    }
}