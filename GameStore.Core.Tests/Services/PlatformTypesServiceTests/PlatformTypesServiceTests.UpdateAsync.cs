using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Models.Server.PlatformTypes;
using GameStore.Core.Models.Server.PlatformTypes.Specifications;
using GameStore.Core.Models.ServiceModels.PlatformTypes;
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
    public async Task UpdateAsync_CorrectValues_NoReturnValue(
        PlatformTypeUpdateModel updateModel,
        PlatformType platform,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        PlatformTypeService sut)
    {
        unitOfWorkMock.Setup(repository => repository.GetEfRepository<PlatformType>().GetSingleOrDefaultBySpecAsync(
                                 It.IsAny<PlatformTypesSpec>()))
                      .ReturnsAsync(platform);

        await sut.UpdateAsync(updateModel);

        platform.Name.Should().Be(updateModel.Name);

        unitOfWorkMock.Verify(
            repository =>
                repository.GetEfRepository<PlatformType>().UpdateAsync(It.IsAny<PlatformType>()),
            Times.Once);

        unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
    }
}