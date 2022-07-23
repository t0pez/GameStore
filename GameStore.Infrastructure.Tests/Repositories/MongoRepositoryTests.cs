using System.Collections.Generic;
using FluentAssertions;
using GameStore.Core.Models.Mongo.Categories;
using GameStore.Core.Models.Mongo.Products;
using GameStore.Core.Models.Mongo.Products.Filters;
using GameStore.Core.Models.Mongo.Products.Specifications;
using GameStore.Infrastructure.Data.Configurations;
using GameStore.Infrastructure.Data.Repositories;
using MongoDB.Driver;
using Xunit;

namespace GameStore.Infrastructure.Tests.Repositories;

public class MongoRepositoryTests
{
    private const string TestDataConnectionString = "mongodb://localhost:27017/Northwind-Test";

    private readonly MongoRepository<Product> _repository;
    private IMongoDatabase _database;

    public MongoRepositoryTests()
    {
        SetMongoTestDatabase();

        DependencyInjectionExtensions.ConfigureNorthwindDatabase(null);
        
        _repository = new MongoRepository<Product>(_database);
    }

    [Fact]
    public async void GetBySpec_NoParameters_ReturnsAllData()
    {
        const int allProductsCount = 77;
        
        var actualResult = await _repository.GetBySpecAsync();

        actualResult.Should().HaveCount(allProductsCount);
    }
    
    [Fact]
    public async void GetBySpec_SpecWithoutInclude_ReturnsCorrectResult()
    {
        const int expectedProductsCount = 24;
        
        var filter = new ProductFilter
        {
            CategoriesIds = new List<int> {1, 2}
        };
        
        var actualResult = await _repository.GetBySpecAsync(new ProductsByFilterSpec(filter));

        actualResult.Should().HaveCount(expectedProductsCount);
    }
    
    [Fact]
    public async void GetFirstOrDefaultBySpec_SpecWithInclude_ReturnsCorrectResult()
    {
        const string expectedGameKey = "game-key";
        const int expectedCategoryId = 2;
        const string expectedCategoryName = "Condiments";
        
        var actualResult = await _repository.GetFirstOrDefaultBySpecAsync(new ProductByGameKeyWithDetailsSpec(expectedGameKey));

        actualResult.GameKey.Should().Be(expectedGameKey);
        actualResult.CategoryId.Should().Be(expectedCategoryId);
        actualResult.Category.Should().NotBeNull().And.Subject.As<Category>().Name.Should().Be(expectedCategoryName);
    }

    private void SetMongoTestDatabase()
    {
        var connection = new MongoUrlBuilder(TestDataConnectionString);
        var client = new MongoClient(TestDataConnectionString);
        
        _database = client.GetDatabase(connection.DatabaseName);
    }
}