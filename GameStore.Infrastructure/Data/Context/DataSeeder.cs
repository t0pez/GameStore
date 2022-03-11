using System;
using System.Collections.Generic;
using GameStore.Core.Models.Games;

namespace GameStore.Infrastructure.Data.Context;

public static class DataSeeder
{
    public static void SeedData(this ApplicationContext context)
    {
        var strategyGenre = new Genre
        {
            Id = Guid.Parse("3FA85F64-0000-0000-0000-000000000001"),
            Name = "Strategy",
            IsDeleted = false,
            Games = new List<Game>(),
            SubGenres = new List<Genre>()
        };
        var rtsGenre = new Genre
        {
            Id = Guid.Parse("3FA85F64-0000-0000-0000-000000000002"),
            Name = "RTS",
            IsDeleted = false,
            Games = new List<Game>(),
            SubGenres = new List<Genre>()
        };
        var cityBuilderGenre = new Genre
        {
            Id = Guid.Parse("3FA85F64-0000-0000-0000-000000000003"),
            Name = "City builder",
            IsDeleted = false,
            Games = new List<Game>(),
            SubGenres = new List<Genre>()
        };

        var pcPlatform = new PlatformType
        {
            Id = Guid.Parse("3FA85F64-0000-0000-0000-000000000001"),
            Name = "PC",
            IsDeleted = false,
            Games = new List<Game>()
        };
        var psPlatform = new PlatformType
        {
            Id = Guid.Parse("3FA85F64-0000-0000-0000-000000000002"),
            Name = "PS5",
            IsDeleted = false,
            Games = new List<Game>()
        };
        var xboxPlatform = new PlatformType
        {
            Id = Guid.Parse("3FA85F64-0000-0000-0000-000000000003"),
            Name = "XBOX Series",
            IsDeleted = false,
            Games = new List<Game>()
        };
        
        context.Add(strategyGenre);
        context.Add(rtsGenre);
        context.Add(cityBuilderGenre);
        context.SaveChanges();
        
        strategyGenre.SubGenres.Add(rtsGenre);
        strategyGenre.SubGenres.Add(cityBuilderGenre);
        
        context.Update(strategyGenre);
        
        context.AddRange(pcPlatform, psPlatform, xboxPlatform);

        context.SaveChanges();
    }
}