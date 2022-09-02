using System.Threading.Tasks;
using AutoFixture.Xunit2;
using GameStore.Core.Models.Server.Publishers;
using GameStore.Core.Models.ServiceModels.Publishers;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using GameStore.Tests.Infrastructure.Attributes;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services.PublisherServiceTests;

public partial class PublisherServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task CreateAsync_CorrectValues_NoReturnValue(
        PublisherCreateModel createModel,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        PublisherService sut)
    {
        await sut.CreateAsync(createModel);

        unitOfWorkMock.Verify(repository => repository.GetEfRepository<Publisher>().AddAsync(It.IsAny<Publisher>()),
                              Times.Once);

        unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(), Times.Once);
    }
}