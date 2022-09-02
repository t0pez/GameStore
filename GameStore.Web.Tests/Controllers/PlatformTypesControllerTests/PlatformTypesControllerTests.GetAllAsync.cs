using System.Collections.Generic;
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
    public async void GetAllAsync_NoParameters_ReturnsCorrectValue(
        List<PlatformType> platforms,
        [Frozen] Mock<IPlatformTypeService> platformServiceMock,
        PlatformTypesController sut)
    {
        platformServiceMock.Setup(service => service.GetAllAsync())
                           .ReturnsAsync(platforms);

        var actualResult = await sut.GetAllAsync();

        actualResult.Result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeAssignableTo<IEnumerable<PlatformTypeListViewModel>>();
    }
}