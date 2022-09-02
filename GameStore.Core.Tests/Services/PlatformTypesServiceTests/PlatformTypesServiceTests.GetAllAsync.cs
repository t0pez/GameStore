using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Models.Server.PlatformTypes;
using GameStore.Core.Models.Server.PlatformTypes.Specifications;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using GameStore.Tests.Infrastructure.Attributes;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services.PlatformTypesServiceTests;

public partial class PlatformTypeServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task GetAllAsync_ReturnsCorrectValues(
        List<PlatformType> platforms,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        PlatformTypeService sut)
    {
        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<PlatformType>()
                                           .GetBySpecAsync(It.IsAny<PlatformTypesSpec>()))
                      .ReturnsAsync(platforms);

        var actualResult = await sut.GetAllAsync();

        actualResult.Should().BeEquivalentTo(platforms);
    }
}