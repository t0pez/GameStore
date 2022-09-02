using System.Threading.Tasks;
using AutoFixture.Xunit2;
using GameStore.Core.Models.Server.Genres;
using GameStore.Core.Models.ServiceModels.Genres;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using GameStore.Tests.Infrastructure.Attributes;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services.GenreServiceTests;

public partial class GenreServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task CreateAsync_CorrectValues_NoReturnValue(
        GenreCreateModel createModel,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        GenreService sut)
    {
        await sut.CreateAsync(createModel);

        unitOfWorkMock.Verify(repository => repository.GetEfRepository<Genre>().AddAsync(It.IsAny<Genre>()),
                              Times.Once);

        unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(), Times.Once);
    }
}