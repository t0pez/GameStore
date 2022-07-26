using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Genres;
using GameStore.Core.Models.PlatformTypes;
using GameStore.Core.Models.Publishers;
using GameStore.Core.Models.RelationalModels;

namespace GameStore.Infrastructure.Data.Context;

internal static class DataSeeder
{
    internal static void SeedData(this ApplicationContext context)
    {
        if (context.Set<Genre>().Any() || context.Set<PlatformType>().Any())
        {
            return;
        }

        var strategyGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Strategy",
            IsDeleted = false,
            Games = new List<GameGenre>(),
            SubGenres = new List<Genre>()
        };
        var rtsGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "RTS",
            IsDeleted = false,
            Games = new List<GameGenre>(),
            ParentId = strategyGenre.Id,
            SubGenres = new List<Genre>()
        };
        var cityBuilderGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "City builder",
            IsDeleted = false,
            Games = new List<GameGenre>(),
            ParentId = strategyGenre.Id,
            SubGenres = new List<Genre>()
        };
        var shooterGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Shooter",
            IsDeleted = false,
            Games = new List<GameGenre>(),
            SubGenres = new List<Genre>()
        };
        var fpsGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "First person shooter",
            IsDeleted = false,
            Games = new List<GameGenre>(),
            ParentId = shooterGenre.Id,
            SubGenres = new List<Genre>()
        };
        var tpsGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Third person shooter",
            IsDeleted = false,
            Games = new List<GameGenre>(),
            ParentId = shooterGenre.Id,
            SubGenres = new List<Genre>()
        };
        var actionGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Action",
            IsDeleted = false,
            Games = new List<GameGenre>(),
            SubGenres = new List<Genre>()
        };

        var pcPlatform = new PlatformType
        {
            Id = Guid.NewGuid(),
            Name = "PC",
            IsDeleted = false,
            Games = new List<GamePlatformType>()
        };
        var psPlatform = new PlatformType
        {
            Id = Guid.NewGuid(),
            Name = "PS5",
            IsDeleted = false,
            Games = new List<GamePlatformType>()
        };
        var xboxPlatform = new PlatformType
        {
            Id = Guid.NewGuid(),
            Name = "XBOX Series",
            IsDeleted = false,
            Games = new List<GamePlatformType>()
        };

        var publisher1 = new Publisher
        {
            Id = Guid.NewGuid(),
            Name = "First publisher",
            HomePage = "Home page for first publisher",
            Description = "Description for first publisher",
            IsDeleted = false
        };
        var publisher2 = new Publisher
        {
            Id = Guid.NewGuid(),
            Name = "Second publisher",
            HomePage = "Home page for second publisher",
            Description = "Description for second publisher",
            IsDeleted = false
        };

        var game1 = new Game
        {
            Id = Guid.NewGuid(),
            Name = "Civilization VI",
            Description = "Description for first game",
            Key = "civilization-6",
            Discontinued = false,
            Price = 60,
            UnitsInStock = 100,
            PublisherName = publisher1.Name,
            File = new byte[] { 0, 1, 0, 1, 0 },
            PublishedAt = DateTime.UtcNow.AddMonths(-3),
            AddedToStoreAt = DateTime.UtcNow.AddMonths(-1),
            IsDeleted = false
        };
        var game2 = new Game
        {
            Id = Guid.NewGuid(),
            Name = "DOOM",
            Description = "Description for first game",
            Key = "doom",
            Discontinued = false,
            Price = 60,
            UnitsInStock = 100,
            PublisherName = publisher2.Name,
            File = new byte[] { 0, 1, 0, 1, 0 },
            PublishedAt = DateTime.UtcNow.AddMonths(-2),
            AddedToStoreAt = DateTime.UtcNow.AddMonths(-1),
            IsDeleted = false
        };

        var game1Genre1 = new GameGenre
        {
            GameId = game1.Id,
            GenreId = strategyGenre.Id
        };
        var game2Genre1 = new GameGenre
        {
            GameId = game2.Id,
            GenreId = shooterGenre.Id
        };
        var game2Genre2 = new GameGenre
        {
            GameId = game2.Id,
            GenreId = fpsGenre.Id
        };

        var game1platform1 = new GamePlatformType
        {
            GameId = game1.Id,
            PlatformId = pcPlatform.Id
        };
        var game1platform2 = new GamePlatformType
        {
            GameId = game1.Id,
            PlatformId = psPlatform.Id
        };
        var game2platform1 = new GamePlatformType
        {
            GameId = game2.Id,
            PlatformId = pcPlatform.Id
        };
        var game2platform2 = new GamePlatformType
        {
            GameId = game2.Id,
            PlatformId = psPlatform.Id
        };
        var game2platform3 = new GamePlatformType
        {
            GameId = game2.Id,
            PlatformId = xboxPlatform.Id
        };

        context.Add(strategyGenre);
        context.Add(rtsGenre);
        context.Add(cityBuilderGenre);
        context.Add(shooterGenre);
        context.Add(fpsGenre);
        context.Add(tpsGenre);
        context.Add(actionGenre);
        context.SaveChanges();

        context.AddRange(pcPlatform, psPlatform, xboxPlatform);
        context.SaveChanges();

        context.Add(publisher1);
        context.Add(publisher2);
        context.SaveChanges();

        context.Add(game1);
        context.Add(game2);
        context.SaveChanges();

        context.Add(game1Genre1);
        context.Add(game2Genre1);
        context.Add(game2Genre2);
        context.SaveChanges();

        context.Add(game1platform1);
        context.Add(game1platform2);
        context.Add(game2platform1);
        context.Add(game2platform2);
        context.Add(game2platform3);
        context.SaveChanges();
    }
}