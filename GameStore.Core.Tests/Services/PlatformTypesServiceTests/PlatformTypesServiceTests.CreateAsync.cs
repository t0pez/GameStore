using System.Threading.Tasks;
using AutoFixture.Xunit2;
using GameStore.Core.Models.Server.PlatformTypes;
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
    public async Task CreateAsync_CorrectValues_NoReturnValue(
        PlatformTypeCreateModel createModel,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        PlatformTypeService sut)
    {
        await sut.CreateAsync(createModel);

        unitOfWorkMock.Verify(
            repository => repository.GetEfRepository<PlatformType>().AddAsync(It.IsAny<PlatformType>()), Times.Once);

        unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(), Times.Once);
    }
}