using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Models.Server.Genres;
using GameStore.Core.Models.Server.Genres.Specifications;
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
    public async Task DeleteAsync_CorrectValues_GenreSoftDeleted(
        Genre genre,
        List<Genre> subGenres,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        GenreService sut)
    {
        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Genre>().GetSingleOrDefaultBySpecAsync(
                                     It.IsAny<GenresSpec>()))
                      .ReturnsAsync(genre);

        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Genre>().GetBySpecAsync(
                                     It.IsAny<GenresSpec>()))
                      .ReturnsAsync(subGenres);

        await sut.DeleteAsync(genre.Id);

        genre.IsDeleted.Should().Be(true);
        subGenres.Select(sg => sg.ParentId).Should().AllBeEquivalentTo(genre.ParentId);

        unitOfWorkMock.Verify(repository => repository.GetEfRepository<Genre>().UpdateAsync(It.IsAny<Genre>()));
        unitOfWorkMock.Verify(work => work.SaveChangesAsync(), Times.Once);
    }
}