using System.Threading.Tasks;
using AutoFixture.Xunit2;
using GameStore.Core.Models.Server.Publishers;
using GameStore.Core.Models.Server.Publishers.Specifications;
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
    public async Task DeleteAsync_CorrectValues_PlatformSoftDeleted(
        Publisher publisher,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        PublisherService sut)
    {
        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Publisher>().GetSingleOrDefaultBySpecAsync(
                                     It.IsAny<PublishersSpec>()))
                      .ReturnsAsync(publisher);

        await sut.DeleteAsync(publisher.Name);

        unitOfWorkMock.Verify(
            repository => repository.GetEfRepository<Publisher>().UpdateAsync(It.IsAny<Publisher>()),
            Times.Once);

        unitOfWorkMock.Verify(work => work.SaveChangesAsync(), Times.Once);
    }
}