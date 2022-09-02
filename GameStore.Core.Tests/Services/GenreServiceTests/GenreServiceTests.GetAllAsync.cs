using System.Collections.Generic;
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
    public async Task GetAllAsync_ReturnsCorrectValues(
        List<Genre> genres,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        GenreService sut)
    {
        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Genre>().GetBySpecAsync(It.IsAny<GenresSpec>()))
                      .ReturnsAsync(genres);

        var actualResult = await sut.GetAllAsync();

        actualResult.Should().BeEquivalentTo(genres);
    }
}