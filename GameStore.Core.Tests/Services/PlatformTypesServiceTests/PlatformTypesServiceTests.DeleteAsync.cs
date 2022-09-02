using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
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
    public async Task DeleteAsync_CorrectValues_PlatformSoftDeleted(
        PlatformType platform,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        PlatformTypeService sut)
    {
        var expectedId = Guid.NewGuid();

        unitOfWorkMock
           .Setup(repository =>
                      repository.GetEfRepository<PlatformType>()
                                .GetSingleOrDefaultBySpecAsync(It.IsAny<PlatformTypesSpec>()))
           .ReturnsAsync(platform);

        await sut.DeleteAsync(expectedId);

        unitOfWorkMock.Verify(
            repository => repository.GetEfRepository<PlatformType>().UpdateAsync(It.IsAny<PlatformType>()), Times.Once);

        unitOfWorkMock.Verify(work => work.SaveChangesAsync(), Times.Once);
    }
}