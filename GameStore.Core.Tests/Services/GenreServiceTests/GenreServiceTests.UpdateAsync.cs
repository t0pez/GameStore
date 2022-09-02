using System.Threading.Tasks;
using AutoFixture.Xunit2;
using GameStore.Core.Models.Server.Genres;
using GameStore.Core.Models.Server.Genres.Specifications;
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
    public async Task UpdateAsync_CorrectValues_NoReturnValue(
        GenreUpdateModel updateModel,
        Genre genre,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        GenreService sut)
    {
        unitOfWorkMock.Setup(repository => repository.GetEfRepository<Genre>().GetSingleOrDefaultBySpecAsync(
                                 It.IsAny<GenresSpec>()))
                      .ReturnsAsync(genre);

        unitOfWorkMock.Setup(repository => repository.GetEfRepository<Genre>().UpdateAsync(
                                 It.Is<Genre>(model => model.Name == genre.Name)))
                      .Verifiable();

        await sut.UpdateAsync(updateModel);

        unitOfWorkMock.Verify(repository => repository.GetEfRepository<Genre>().UpdateAsync(It.IsAny<Genre>()),
                              Times.Once);

        unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
    }
}