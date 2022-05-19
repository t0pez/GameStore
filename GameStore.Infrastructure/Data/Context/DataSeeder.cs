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
        if(context.Set<Genre>().Any() || context.Set<PlatformType>().Any())
            return;
        
        var strategyGenre = new Genre
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7601"),
            Name = "Strategy",
            IsDeleted = false,
            Games = new List<GameGenre>(),
            SubGenres = new List<Genre>()
        };
        var rtsGenre = new Genre
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7602"),
            Name = "RTS",
            IsDeleted = false,
            Games = new List<GameGenre>(),
            ParentId = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7601"),
            SubGenres = new List<Genre>()
        };
        var cityBuilderGenre = new Genre
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7603"),
            Name = "City builder",
            IsDeleted = false,
            Games = new List<GameGenre>(),
            ParentId = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7601"),
            SubGenres = new List<Genre>()
        };
        var shooterGenre = new Genre
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7604"),
            Name = "Shooter",
            IsDeleted = false,
            Games = new List<GameGenre>(),
            SubGenres = new List<Genre>()
        };
        var actionGenre = new Genre
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7605"),
            Name = "Shooter",
            IsDeleted = false,
            Games = new List<GameGenre>(),
            SubGenres = new List<Genre>()
        };

        var pcPlatform = new PlatformType
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7611"),
            Name = "PC",
            IsDeleted = false,
            Games = new List<GamePlatformType>()
        };
        var psPlatform = new PlatformType
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7612"),
            Name = "PS5",
            IsDeleted = false,
            Games = new List<GamePlatformType>()
        };
        var xboxPlatform = new PlatformType
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7613"),
            Name = "XBOX Series",
            IsDeleted = false,
            Games = new List<GamePlatformType>()
        };
        
        var publisher1 = new Publisher
        {
            Id = Guid.Parse("6fd6d158-7ff1-472a-b97c-08da067d7613"),
            Name = "First publisher",
            HomePage = "Home page for first publisher",
            Description = "Description for first publisher",
            IsDeleted = false
        };
        var publisher2 = new Publisher
        {
            Id = Guid.Parse("6fd6d158-7ff2-472a-b97c-08da067d7613"),
            Name = "Second publisher",
            HomePage = "Home page for second publisher",
            Description = "Description for second publisher",
            IsDeleted = false
        };

        var game1 = new Game
        {
            Id = Guid.Parse("6fd6d158-7ff1-012a-b97c-08da067d7613"),
            Name = "Civilization VI",
            Description = "Description for first game",
            Key = "civilization-6",
            Discontinued = false,
            Price = 60,
            UnitsInStock = 100,
            PublisherId = Guid.Parse("6fd6d158-7ff1-472a-b97c-08da067d7613"),
            Genres = new List<GameGenre>
            {
                new()
                {
                    GameId = Guid.Parse("6fd6d158-7ff1-012a-b97c-08da067d7613"),
                    GenreId = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7601")
                },
                new()
                {
                    GameId = Guid.Parse("6fd6d158-7ff1-012a-b97c-08da067d7613"),
                    GenreId = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7603")
                }
            },
            Platforms = new List<GamePlatformType>
            {
                new()
                {
                    GameId = Guid.Parse("6fd6d158-7ff1-012a-b97c-08da067d7613"),
                    PlatformId = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7611")
                },
                new()
                {
                    GameId = Guid.Parse("6fd6d158-7ff1-012a-b97c-08da067d7613"),
                    PlatformId = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7612")
                }
            },
            File = new byte[] { 0, 1, 0, 1, 0 },
            IsDeleted = false
        };
        
        context.Add(strategyGenre);
        context.Add(rtsGenre);
        context.Add(cityBuilderGenre);
        context.Add(shooterGenre);
        context.Add(actionGenre);
        context.SaveChanges();
        
        context.AddRange(pcPlatform, psPlatform, xboxPlatform);
        context.SaveChanges();

        context.Add(publisher1);
        context.Add(publisher2);
        context.SaveChanges();

        context.Add(game1);
        context.SaveChanges();
    }
}