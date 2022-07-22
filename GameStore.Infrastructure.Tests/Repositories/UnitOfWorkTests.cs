using System;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.RelationalModels;
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
        _unitOfWork = new UnitOfWork(_context);
    }

    [Fact]
    public void GetRepository_Game()
    {
        var actualResult = _unitOfWork.GetEfRepository<Game>();

        Assert.NotNull(actualResult);
    }

    [Fact]
    public void GetRepository_GameGenre()
    {
        var actualResult = _unitOfWork.GetEfRepository<GameGenre>();

        Assert.NotNull(actualResult);
    }

    [Fact]
    public void GetRepository_Comment()
    {
        var actualResult = _unitOfWork.GetEfRepository<Comment>();

        Assert.NotNull(actualResult);
    }

    [Fact]
    public void GetRepository_NotCorrectModel_RepositoryMethodThrowsException()
    {
        var repository = _unitOfWork.GetEfRepository<NotExistingModel>();

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