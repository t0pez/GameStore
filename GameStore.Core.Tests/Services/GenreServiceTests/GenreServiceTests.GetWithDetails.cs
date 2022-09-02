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
    public async Task GetWithDetailsAsync_ReturnsCorrectValues(
        Genre genre,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        GenreService sut)
    {
        unitOfWorkMock.Setup(repository => repository.GetEfRepository<Genre>().GetSingleOrDefaultBySpecAsync(
                                 It.IsAny<GenresSpec>()))
                      .ReturnsAsync(genre);

        var actualResult = await sut.GetByIdAsync(genre.Id);

        actualResult.Should().Be(genre);
    }
}