using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.Core.Models.Games;

namespace GameStore.Infrastructure.Data.Context;

public static class DataSeeder
{
    public static void SeedData(this ApplicationContext context)
    {
        if(context.Set<Genre>().Any() || context.Set<PlatformType>().Any())
            return;
        
        var strategyGenre = new Genre
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7601"),
            Name = "Strategy",
            IsDeleted = false,
            Games = new List<Game>(),
            SubGenres = new List<Genre>()
        };
        var rtsGenre = new Genre
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7602"),
            Name = "RTS",
            IsDeleted = false,
            Games = new List<Game>(),
            SubGenres = new List<Genre>()
        };
        var cityBuilderGenre = new Genre
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7603"),
            Name = "City builder",
            IsDeleted = false,
            Games = new List<Game>(),
            SubGenres = new List<Genre>()
        };

        var pcPlatform = new PlatformType
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7611"),
            Name = "PC",
            IsDeleted = false,
            Games = new List<Game>()
        };
        var psPlatform = new PlatformType
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7612"),
            Name = "PS5",
            IsDeleted = false,
            Games = new List<Game>()
        };
        var xboxPlatform = new PlatformType
        {
            Id = Guid.Parse("6fd6d158-7ffd-472a-b97c-08da067d7613"),
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