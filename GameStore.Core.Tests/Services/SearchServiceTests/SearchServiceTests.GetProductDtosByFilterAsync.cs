using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.Specification;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Core.Models.Mongo.Products;
using GameStore.Core.Models.Mongo.Products.Specifications;
using GameStore.Core.Models.Mongo.Suppliers;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.Games.Specifications;
using GameStore.Core.Models.Server.Genres;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using GameStore.Tests.Infrastructure.Attributes;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services.SearchServiceTests;

public partial class SearchServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task GetProductDtosByFilterAsync_ReturnsCorrectValues(
        AllProductsFilter filter,
        List<Game> games,
        List<Genre> genres,
        List<Product> products,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        SearchService sut)
    {
        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Game>().GetBySpecAsync(It.IsAny<GamesByFilterSpec>()))
                      .ReturnsAsync(games);

        unitOfWorkMock.Setup(repository => repository.GetMongoRepository<Product>()
                                                     .GetBySpecAsync(It.IsAny<ProductsByFilterSpec>()))
                      .ReturnsAsync(products);

        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Game>()
                                           .SelectBySpecAsync(It.IsAny<ISpecification<Game, string>>()))
                      .ReturnsAsync(games.Select(game => game.Key).ToList);

        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Genre>()
                                           .SelectBySpecAsync(It.IsAny<ISpecification<Genre, int>>()))
                      .ReturnsAsync(new List<int>());

        unitOfWorkMock.Setup(repository =>
                                 repository.GetMongoRepository<Supplier>()
                                           .SelectBySpecAsync(It.IsAny<ISpecification<Supplier, int>>()))
                      .ReturnsAsync(new List<int>());

        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Genre>().GetBySpecAsync(It.IsAny<ISpecification<Genre>>()))
                      .ReturnsAsync(genres);

        var actualResult = await sut.GetProductDtosByFilterAsync(filter);

        var expectedCount = filter.Skip + filter.Take < games.Count + products.Count
            ? games.Count + products.Count
            : 0;

        actualResult.Result.Should().HaveCount(expectedCount);
    }
}