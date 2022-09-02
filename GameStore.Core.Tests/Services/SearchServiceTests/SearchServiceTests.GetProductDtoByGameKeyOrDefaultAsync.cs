using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Models.Mongo.Products;
using GameStore.Core.Models.Mongo.Products.Specifications;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.Games.Specifications;
using GameStore.Core.Models.Server.Genres;
using GameStore.Core.Models.Server.Genres.Specifications;
using GameStore.Core.Models.Server.Publishers;
using GameStore.Core.Models.Server.Publishers.Specifications;
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
    public async Task GetProductDtoByGameKeyOrDefaultAsync_ServerEntityFound_IncludesPublisher(
        Game game,
        Publisher publisher,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        SearchService sut)
    {
        unitOfWorkMock.Setup(repository => repository.GetEfRepository<Game>()
                                                     .GetSingleOrDefaultBySpecAsync(
                                                          It.IsAny<GamesSpec>()))
                      .ReturnsAsync(game);

        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Publisher>().GetSingleOrDefaultBySpecAsync(
                                     It.IsAny<PublishersSpec>()))
                      .ReturnsAsync(publisher);

        var actualResult = await sut.GetProductDtoByGameKeyOrDefaultAsync(game.Key);

        actualResult.Key.Should().Be(game.Key);
        actualResult.PublisherName.Should().Be(game.PublisherName);
    }

    [Theory]
    [AutoMoqData]
    public async Task GetProductDtoByGameKeyOrDefaultAsync_MongoEntityFound_IncludesSupplier(
        Product product,
        Genre genre,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        SearchService sut)
    {
        unitOfWorkMock.Setup(repository => repository.GetEfRepository<Game>()
                                                     .GetSingleOrDefaultBySpecAsync(It.IsAny<GamesSpec>()))
                      .ReturnsAsync(() => null!);

        unitOfWorkMock.Setup(repository => repository.GetMongoRepository<Product>()
                                                     .GetFirstOrDefaultBySpecAsync(It.IsAny<ProductsSpec>()))
                      .ReturnsAsync(product);

        unitOfWorkMock.Setup(repository => repository.GetEfRepository<Genre>().GetSingleOrDefaultBySpecAsync(
                                 It.IsAny<GenresSpec>()))
                      .ReturnsAsync(genre);

        var actualResult = await sut.GetProductDtoByGameKeyOrDefaultAsync(product.GameKey);

        actualResult.Key.Should().Be(product.GameKey);
        actualResult.Publisher.Name.Should().Be(product.Supplier.CompanyName);
        actualResult.Genres.Should().Contain(g => g.CategoryId == genre.CategoryId);
    }
}