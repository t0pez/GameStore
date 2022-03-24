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
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationContext _context;

    public UnitOfWorkTests()
    {
        _context = GetInMemoryContext();
        _unitOfWork = new UnitOfWork(_context);
    }

    [Fact]
    public void GetRepository_Game()
    {
        var actualResult = _unitOfWork.GetRepository<Game>();
        
        Assert.NotNull(actualResult);
    }
    
    [Fact]
    public void GetRepository_GameGenre()
    {
        var actualResult = _unitOfWork.GetRepository<GameGenre>();
        
        Assert.NotNull(actualResult);
    }
    
    [Fact]
    public void GetRepository_Comment()
    {
        var actualResult = _unitOfWork.GetRepository<Comment>();
        
        Assert.NotNull(actualResult);
    }

    [Fact]
    public void GetRepository_NotCorrectModel_ThrowsException()
    {
        var function = () =>
                       {
                           _unitOfWork.GetRepository<NotExistingModel>();
                       };

        Assert.Throws<InvalidOperationException>(function);
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