using System;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Publishers;
using GameStore.Infrastructure.Data.Context;
using GameStore.Infrastructure.Data.Repositories;
using GameStore.Infrastructure.Tests.TestData.Repositories;
using GameStore.SharedKernel.Interfaces.DataAccess;
using GameStore.SharedKernel.Specifications;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GameStore.Infrastructure.Tests.Repositories;

public class RepositoryTests
{
    private readonly IRepository<Game> _gameRepository;
    private readonly ApplicationContext _context;

    public RepositoryTests()
    {
        var context = GetInMemoryContext();
        
        SetTestContextData(context);
        context.ChangeTracker.Clear();
        
        _context = context;
        _gameRepository = new Repository<Game>(_context);
    }

    [Fact]
    public async void GetBySpecAsync_NullSpec_ReturnsAllModels()
    {
        const int expectedResultCount = 5;

        var actualResult = await _gameRepository.GetBySpecAsync();
        var actualResultCount = actualResult.Count;

        Assert.Equal(expectedResultCount, actualResultCount);
    }

    [Theory] 
    [GetBySpecAsyncData]
    public async void GetBySpecAsync_NotNullSpec_ReturnsCorrectModels(
        SafeDeleteSpec<Game> spec, int expectedCount)
    {
        var actualResult = await _gameRepository.GetBySpecAsync(spec);
        var actualCount = actualResult.Count;
        
        Assert.Equal(expectedCount, actualCount);
    }
    
    [Theory]
    [GetSingleBySpecWithResultData]
    public async void GetSingleBySpecAsync_ExistingModel_ReturnsCorrectModels(
        SafeDeleteSpec<Game> spec, Guid expectedId)
    {
        var actualResult = await _gameRepository.GetSingleOrDefaultBySpecAsync(spec);
        var actualResultId = actualResult.Id;
        
        Assert.Equal(expectedId,actualResultId);
    }
    
    [Theory]
    [GetSingleBySpecWithoutResultData]
    public async void GetSingleBySpecAsync_NotExistingModel_ReturnsNull(
        SafeDeleteSpec<Game> spec)
    {
        const Game expectedResult = null!;

        var actualResult = await _gameRepository.GetSingleOrDefaultBySpecAsync(spec);
        
        Assert.Same(expectedResult, actualResult);
    }

    [Theory]
    [AddAsyncCorrectModelData]
    public async void AddAsync_CorrectModel(
        Game model)
    {
        const int expectedCount = 6;
        
        await _gameRepository.AddAsync(model);
        await _context.SaveChangesAsync();
        var actualResult = await _gameRepository.GetBySpecAsync();
        var actualResultCount = actualResult.Count;

        Assert.Equal(expectedCount, actualResultCount);
    }
    
    [Theory]
    [AddAsyncIncorrectModelData]
    public async void AddAsync_ExistingModel_SaveChangesMethodThrowsException(
        Game model)
    {
        await _gameRepository.AddAsync(model);
        var saveChangesAfterAddMethod = async () =>
                        {
                            await _context.SaveChangesAsync();
                        };

        await Assert.ThrowsAsync<ArgumentException>(saveChangesAfterAddMethod);
    }
    
    [Theory]
    [UpdateAsyncIncorrectModelData]
    public async void UpdateAsync_NotExistingModel_SaveChangesThrowsException(
        Game updated)
    {
        await _gameRepository.UpdateAsync(updated);
        var saveChangesAfterUpdateMethod = async () =>
                           {
                               await _context.SaveChangesAsync();
                           };
        
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(saveChangesAfterUpdateMethod);
    }

    [Theory]
    [DeleteAsyncCorrectModelData]
    public async void DeleteAsync_ExistingModel(Game model)
    {
        const int expectedCount = 4;

        await _gameRepository.DeleteAsync(model);
        await _context.SaveChangesAsync();
        var actualResult = await _gameRepository.GetBySpecAsync();
        var actualResultCount = actualResult.Count;
        
        Assert.Equal(expectedCount, actualResultCount);
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

    private void SetTestContextData(ApplicationContext context)
    {
        var game1 = new Game
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601"),
            Name = "First game",
            Key = "first-game",
            Description = "First description",
            File = new byte[] { 0, 0, 0, 1 },
            IsDeleted = false,
            PublisherId = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7111"),
            Publisher = new Publisher { Id = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7111") }
        };
        var game2 = new Game
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601"),
            Name = "Second game",
            Key = "second-game",
            Description = "Second description",
            File = new byte[] { 0, 0, 0, 2 }, 
            IsDeleted = false,
            PublisherId = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7222"),
            Publisher = new Publisher { Id = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7222") }
        };
        var game3 = new Game
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b973-08da067d7601"),
            Name = "Third game",
            Key = "Third-game",
            Description = "Third description",
            File = new byte[] { 0, 0, 0, 3 }, 
            IsDeleted = false,
            PublisherId = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7333"),
            Publisher = new Publisher { Id = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7333") }
        };
        var game4 = new Game
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601"),
            Name = "Fourth game",
            Key = "fourth-game",
            Description = "Fourth description",
            File = new byte[] { 0, 0, 0, 4 }, 
            IsDeleted = false,
            PublisherId = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7111"),
            Publisher = new Publisher { Id = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7444") }
        };

        context.AddRange(game1, game2, game3, game4);
        context.SaveChanges();
    }

}