using System;
using GameStore.Core.Models.Mongo.Products;
using GameStore.Core.Models.Mongo.Suppliers;
using GameStore.Core.Models.Server.Comments;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.RelationalModels;
using GameStore.Infrastructure.Data.Context;
using GameStore.Infrastructure.Data.Repositories;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GameStore.Infrastructure.Tests.Repositories;

public class UnitOfWorkTests
{
    private readonly ApplicationContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        _context = GetInMemoryContext();
        _unitOfWork = new UnitOfWork(_context, "mongodb://localhost:27017/Northwind-Test");
    }

    [Fact]
    public void GetEfRepository_Game()
    {
        var actualResult = _unitOfWork.GetEfRepository<Game>();

        Assert.NotNull(actualResult);
    }

    [Fact]
    public void GetEfRepository_GameGenre()
    {
        var actualResult = _unitOfWork.GetEfRepository<GameGenre>();

        Assert.NotNull(actualResult);
    }

    [Fact]
    public void GetEfRepository_Comment()
    {
        var actualResult = _unitOfWork.GetEfRepository<Comment>();

        Assert.NotNull(actualResult);
    }

    [Fact]
    public void GetMongoRepository_Product()
    {
        var actualResult = _unitOfWork.GetMongoRepository<Product>();

        Assert.NotNull(actualResult);
    }

    [Fact]
    public void GetMongoRepository_Supplier()
    {
        var actualResult = _unitOfWork.GetMongoRepository<Supplier>();

        Assert.NotNull(actualResult);
    }

    [Fact]
    public void GetEfRepository_NotCorrectModel_RepositoryMethodThrowsException()
    {
        var repository = _unitOfWork.GetEfRepository<NotExistingModel>();

        var function = async () => { await repository.AnyAsync(null); };

        Assert.ThrowsAsync<InvalidOperationException>(function);
    }

    [Fact]
    public void GetMongoRepository_NotCorrectModel_RepositoryMethodThrowsException()
    {
        var repository = _unitOfWork.GetMongoRepository<NotExistingModel>();

        var function = async () => { await repository.AnyAsync(null); };

        Assert.ThrowsAsync<InvalidOperationException>(function);
    }

    private ApplicationContext GetInMemoryContext()
    {
        var dbOptionBuilder = new DbContextOptionsBuilder<ApplicationContext>();

        dbOptionBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString())
                       .EnableDetailedErrors()
                       .EnableSensitiveDataLogging();

        var context = new ApplicationContext(dbOptionBuilder.Options);

        return context;
    }
}

public class NotExistingModel
{
}