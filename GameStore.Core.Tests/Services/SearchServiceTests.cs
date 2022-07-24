using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.Core.Models.Games.Specifications.Filters;
using GameStore.Core.Models.Genres;
using GameStore.Core.Models.Genres.Specifications;
using GameStore.Core.Models.Mongo.Products;
using GameStore.Core.Models.Mongo.Products.Filters;
using GameStore.Core.Models.Mongo.Products.Specifications;
using GameStore.Core.Models.Mongo.Suppliers;
using GameStore.Core.Models.Mongo.Suppliers.Specifications;
using GameStore.Core.Models.Publishers;
using GameStore.Core.Models.Publishers.Specifications;
using GameStore.Core.Models.ServiceModels.Enums;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services;

public class SearchServiceTests
{
    private readonly Mock<IRepository<Game>> _gameRepoMock;
    private readonly Mock<IRepository<Genre>> _genreRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepository<Product>> _productRepoMock;
    private readonly Mock<IRepository<Publisher>> _publisherRepoMock;
    private readonly SearchService _searchService;
    private readonly Mock<IRepository<Supplier>> _supplierRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public SearchServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        _gameRepoMock = new Mock<IRepository<Game>>();
        _productRepoMock = new Mock<IRepository<Product>>();
        _genreRepoMock = new Mock<IRepository<Genre>>();
        _publisherRepoMock = new Mock<IRepository<Publisher>>();
        _supplierRepoMock = new Mock<IRepository<Supplier>>();

        _unitOfWorkMock.Setup(unit => unit.GetEfRepository<Game>())
                       .Returns(_gameRepoMock.Object);
        _unitOfWorkMock.Setup(unit => unit.GetMongoRepository<Product>())
                       .Returns(_productRepoMock.Object);
        _unitOfWorkMock.Setup(unit => unit.GetEfRepository<Genre>())
                       .Returns(_genreRepoMock.Object);
        _unitOfWorkMock.Setup(unit => unit.GetEfRepository<Publisher>())
                       .Returns(_publisherRepoMock.Object);
        _unitOfWorkMock.Setup(unit => unit.GetMongoRepository<Supplier>())
                       .Returns(_supplierRepoMock.Object);

        _searchService = new SearchService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async void GetProductDtosByFilterAsync_ReturnsCorrectValues()
    {
        const int expectedCount = 6;

        var filter = new AllProductsFilter { CurrentPage = 1, PageSize = 10 };
        var gameFilter = new GameSearchFilter { CurrentPage = 1, PageSize = 10 };
        var productFilter = new ProductFilter();

        var serverGameKeys = new List<string> { "1", "2", "3" };
        var categoriesIds = new List<int>();
        var suppliersIds = new List<int>();

        var games = new List<Game> { new() { Key = "1" }, new() { Key = "2" }, new() { Key = "3" } };
        var mappedGames = new List<ProductDto> { new() { Key = "1" }, new() { Key = "2" }, new() { Key = "3" } };

        var products = new List<Product> { new() { GameKey = "4" }, new() { GameKey = "5" }, new() { GameKey = "6" } };
        var mappedProducts = new List<ProductDto> { new() { Key = "4" }, new() { Key = "5" }, new() { Key = "6" } };

        _mapperMock.Setup(mapper => mapper.Map<GameSearchFilter>(filter)).Returns(gameFilter);
        _mapperMock.Setup(mapper => mapper.Map<ProductFilter>(filter)).Returns(productFilter);
        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<ProductDto>>(games)).Returns(mappedGames);
        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<ProductDto>>(products)).Returns(mappedProducts);

        _gameRepoMock.Setup(repository =>
                                repository.GetBySpecAsync(It.Is<GamesByFilterSpec>(spec => spec.Filter == gameFilter)))
                     .ReturnsAsync(games);
        _productRepoMock.Setup(repository =>
                                   repository.GetBySpecAsync(
                                       It.Is<ProductsByFilterSpec>(spec => spec.Filter == productFilter)))
                        .ReturnsAsync(products);
        _gameRepoMock.Setup(repository => repository.SelectBySpecAsync(It.IsAny<AllGamesSelectGameKeySpec>()))
                     .ReturnsAsync(serverGameKeys);
        _genreRepoMock.Setup(repository =>
                                 repository.SelectBySpecAsync(It.IsAny<GenreByGenresIdsSelectCategoryIdSpec>()))
                      .ReturnsAsync(categoriesIds);
        _supplierRepoMock.Setup(repository =>
                                    repository.SelectBySpecAsync(It.IsAny<SuppliersByNamesSelectSupplierIdSpec>()))
                         .ReturnsAsync(suppliersIds);

        var actualResult = await _searchService.GetProductDtosByFilterAsync(filter);

        actualResult.Result.Should().HaveCount(expectedCount);
    }

    [Fact]
    public async void GetProductDtoByGameKeyOrDefaultAsync_ServerEntityFound_IncludesPublisher()
    {
        const string gameKey = "game-key";
        const string publisherName = "Publisher Name";
        var game = new Game { Key = gameKey, PublisherName = publisherName, Database = Database.Server };
        var productDto = new ProductDto { Key = gameKey, PublisherName = publisherName, Database = Database.Server };
        var publisher = new Publisher { Name = publisherName, Database = Database.Server };
        var publisherDto = new PublisherDto { Name = publisherName, Database = Database.Server };

        _gameRepoMock.Setup(repository =>
                                repository.GetSingleOrDefaultBySpecAsync(
                                    It.Is<GameByKeyWithDetailsSpec>(spec => spec.Key == gameKey)))
                     .ReturnsAsync(game);
        _publisherRepoMock.Setup(repository =>
                                     repository.GetSingleOrDefaultBySpecAsync(
                                         It.Is<PublisherByNameSpec>(spec => spec.Name == publisherName)))
                          .ReturnsAsync(publisher);

        _mapperMock.Setup(mapper => mapper.Map<ProductDto>(game)).Returns(productDto);
        _mapperMock.Setup(mapper => mapper.Map<PublisherDto>(publisher)).Returns(publisherDto);

        var actualResult = await _searchService.GetProductDtoByGameKeyOrDefaultAsync(gameKey);

        actualResult.Key.Should().Be(gameKey);
        actualResult.Publisher.Should().Be(publisherDto);
    }

    [Fact]
    public async void GetProductDtoByGameKeyOrDefaultAsync_MongoEntityFound_IncludesSupplier()
    {
        const string gameKey = "game-key";
        const int supplierId = 1;
        const int categoryId = 1;

        Game game = null!;
        var genre = new Genre { CategoryId = categoryId };
        var supplier = new Supplier { SupplierId = supplierId, Database = Database.Mongo };
        var product = new Product
        {
            GameKey = gameKey, CategoryId = categoryId, Supplier = supplier, SupplierId = supplierId,
            Database = Database.Mongo
        };
        var publisherDto = new PublisherDto { Database = Database.Mongo };
        var productDto = new ProductDto { Key = gameKey, Publisher = publisherDto, Database = Database.Mongo };

        _gameRepoMock.Setup(repository =>
                                repository.GetSingleOrDefaultBySpecAsync(
                                    It.Is<GameByKeyWithDetailsSpec>(spec => spec.Key == gameKey)))
                     .ReturnsAsync(game);
        _productRepoMock.Setup(repository =>
                                   repository.GetFirstOrDefaultBySpecAsync(
                                       It.Is<ProductByGameKeyWithDetailsSpec>(spec => spec.GameKey == gameKey)))
                        .ReturnsAsync(product);
        _genreRepoMock.Setup(repository =>
                                 repository.GetSingleOrDefaultBySpecAsync(
                                     It.Is<GenreByCategoryIdSpec>(spec => spec.CategoryId == categoryId)))
                      .ReturnsAsync(genre);

        _mapperMock.Setup(mapper => mapper.Map<ProductDto>(product)).Returns(productDto);

        var actualResult = await _searchService.GetProductDtoByGameKeyOrDefaultAsync(gameKey);

        actualResult.Key.Should().Be(gameKey);
        actualResult.Publisher.Should().Be(publisherDto);
        actualResult.Genres.Should().Contain(g => g.CategoryId == categoryId);
    }
}