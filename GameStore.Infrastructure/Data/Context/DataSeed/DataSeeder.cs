using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using GameStore.Core.Extensions;
using GameStore.Core.Helpers.GameKeyGeneration;
using GameStore.Core.Models.Constants;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.Genres;
using GameStore.Core.Models.Server.PlatformTypes;
using GameStore.Core.Models.Server.Publishers;
using GameStore.Core.Models.Server.RelationalModels;
using GameStore.Core.Models.Server.Users;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Data.Context.DataSeed;

public static class DataSeeder
{
    public static void SeedData(this ModelBuilder modelBuilder)
    {
        var publishers = GeneratePublishers();
        var genres = GenerateGenres();
        var platforms = GeneratePlatforms();

        var games = GenerateGames(publishers);

        var gameGenres = GenerateGameGenres(games, genres);
        var gamePlatforms = GenerateGamePlatforms(games, platforms);

        var users = GenerateUsers();

        modelBuilder.Entity<Game>().HasData(games);
        modelBuilder.Entity<Genre>().HasData(genres);
        modelBuilder.Entity<PlatformType>().HasData(platforms);
        modelBuilder.Entity<Publisher>().HasData(publishers);
        modelBuilder.Entity<GameGenre>().HasData(gameGenres);
        modelBuilder.Entity<GamePlatformType>().HasData(gamePlatforms);
        modelBuilder.Entity<User>().HasData(users);
    }

    private static List<Publisher> GeneratePublishers()
    {
        var publisherFaker = new Faker<Publisher>()
                            .RuleFor(publisher => publisher.Id, faker => faker.Random.Guid())
                            .RuleFor(publisher => publisher.Name, faker => faker.Company.CompanyName())
                            .RuleFor(publisher => publisher.Description, faker => faker.Lorem.Paragraphs())
                            .RuleFor(publisher => publisher.HomePage, faker => faker.Internet.Url())
                            .RuleFor(publisher => publisher.Address, faker => faker.Address.FullAddress())
                            .RuleFor(publisher => publisher.City, faker => faker.Address.City())
                            .RuleFor(publisher => publisher.Country, faker => faker.Address.Country())
                            .RuleFor(publisher => publisher.ContactName, faker => faker.Person.FullName)
                            .RuleFor(publisher => publisher.ContactTitle, faker => faker.Lorem.Sentence())
                            .RuleFor(publisher => publisher.Phone, faker => faker.Phone.PhoneNumber())
                            .RuleFor(publisher => publisher.Fax, faker => faker.Random.Long().ToString())
                            .RuleFor(publisher => publisher.PostalCode, faker => faker.Address.ZipCode())
                            .RuleFor(publisher => publisher.Region, faker => faker.Address.State());

        var result = publisherFaker.Generate(100)
                                   .DistinctBy(publisher => publisher.Name)
                                   .ToList();

        return result;
    }

    private static List<Game> GenerateGames(List<Publisher> publishers)
    {
        var gameFaker = new Faker<Game>()
                       .RuleFor(game => game.Id, faker => faker.Random.Guid())
                       .RuleFor(game => game.Name, faker => faker.Commerce.ProductName())
                       .RuleFor(game => game.Key, (_, game) => GameKeyGenerator.GenerateGameKey(game.Name))
                       .RuleFor(game => game.Description, faker => faker.Commerce.ProductDescription())
                       .RuleFor(game => game.Price, faker => faker.Random.Decimal(10m, 60m))
                       .RuleFor(game => game.File, faker => faker.Random.Bytes(1000))
                       .RuleFor(game => game.UnitsInStock, faker => faker.Random.Int(10, 300))
                       .RuleFor(game => game.Discontinued, faker => faker.Random.Bool())
                       .RuleFor(game => game.AddedToStoreAt,
                                faker => faker.Date.Between(DateTime.UtcNow.AddMonths(-6), DateTime.UtcNow))
                       .RuleFor(game => game.PublishedAt,
                                faker => faker.Date.Between(DateTime.UtcNow.AddMonths(-6), DateTime.UtcNow))
                       .RuleFor(game => game.Views, faker => faker.Random.Int(0, 1500))
                       .RuleFor(game => game.QuantityPerUnit, faker => faker.Commerce.ProductMaterial())
                       .RuleFor(game => game.PublisherName, faker => faker.PickRandom(publishers).Name);

        var games = gameFaker.Generate(500)
                             .DistinctBy(game => game.Key)
                             .ToList();

        return games;
    }

    private static List<Genre> GenerateGenres()
    {
        var strategyGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Strategy",
            IsDeleted = false
        };

        var rtsGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "RTS",
            IsDeleted = false,
            ParentId = strategyGenre.Id
        };

        var tbsGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "TBS",
            IsDeleted = false,
            ParentId = strategyGenre.Id
        };

        var rpgGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Shooter",
            IsDeleted = false
        };

        var sportsGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Sports",
            IsDeleted = false
        };

        var racesGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Races",
            IsDeleted = false
        };

        var rallyGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Rally",
            IsDeleted = false,
            ParentId = racesGenre.Id
        };

        var arcadeGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Arcade",
            IsDeleted = false,
            ParentId = racesGenre.Id
        };

        var formulaGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Formula",
            IsDeleted = false,
            ParentId = racesGenre.Id
        };

        var offRoadGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Off-Road",
            IsDeleted = false,
            ParentId = racesGenre.Id
        };

        var actionGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Action",
            IsDeleted = false
        };

        var fpsGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "First person shooter",
            IsDeleted = false,
            ParentId = actionGenre.Id
        };

        var tpsGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Third person shooter",
            IsDeleted = false,
            ParentId = actionGenre.Id
        };

        var adventureGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Adventure",
            IsDeleted = false
        };

        var puzzleGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Puzzle & Skill",
            IsDeleted = false
        };

        var miscGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Misc",
            IsDeleted = false
        };

        var genres = new List<Genre>
        {
            strategyGenre,
            rtsGenre,
            tbsGenre,
            rpgGenre,
            sportsGenre,
            racesGenre,
            rallyGenre,
            arcadeGenre,
            formulaGenre,
            offRoadGenre,
            actionGenre,
            fpsGenre,
            tpsGenre,
            adventureGenre,
            puzzleGenre,
            miscGenre
        };

        return genres;
    }

    private static List<PlatformType> GeneratePlatforms()
    {
        var mobilePlatform = new PlatformType
        {
            Id = Guid.NewGuid(),
            Name = "Mobile",
            IsDeleted = false
        };

        var browserPlatform = new PlatformType
        {
            Id = Guid.NewGuid(),
            Name = "Browser",
            IsDeleted = false
        };

        var desktopPlatform = new PlatformType
        {
            Id = Guid.NewGuid(),
            Name = "Desktop",
            IsDeleted = false
        };

        var consolePlatform = new PlatformType
        {
            Id = Guid.NewGuid(),
            Name = "Console",
            IsDeleted = false
        };

        var platforms = new List<PlatformType>
        {
            mobilePlatform,
            browserPlatform,
            desktopPlatform,
            consolePlatform
        };

        return platforms;
    }

    private static List<GameGenre> GenerateGameGenres(List<Game> games, List<Genre> genres)
    {
        var gameGenreFaker = new Faker<GameGenre>()
                            .RuleFor(gg => gg.GameId, faker => faker.PickRandom(games).Id)
                            .RuleFor(gg => gg.GenreId, faker => faker.PickRandom(genres).Id)
                            .Ignore(gg => gg.Game)
                            .Ignore(gg => gg.Genre);

        var gameGenres = gameGenreFaker.Generate(1500)
                                       .DistinctBy(gg => new { gameId = gg.GameId, genreId = gg.GenreId })
                                       .ToList();

        return gameGenres;
    }

    private static List<GamePlatformType> GenerateGamePlatforms(List<Game> games, List<PlatformType> platforms)
    {
        var gamePlatformFaker = new Faker<GamePlatformType>()
                               .RuleFor(gg => gg.GameId, faker => faker.PickRandom(games).Id)
                               .RuleFor(gg => gg.PlatformId, faker => faker.PickRandom(platforms).Id)
                               .Ignore(gg => gg.Game)
                               .Ignore(gg => gg.Platform);

        var gamePlatforms = gamePlatformFaker.Generate(1500)
                                             .DistinctBy(gp => new { gameId = gp.GameId, platformId = gp.PlatformId })
                                             .ToList();

        return gamePlatforms;
    }

    private static List<User> GenerateUsers()
    {
        var admin = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@example.com",
            UserName = "admin",
            Role = Roles.Administrator,
            PasswordHash = "AJNgcVh84AlNw+NXLkor//tJ78pAmKqyuIoJ+D7cKhbrYXqNrYKQ8PQV1Ow/s+THgw=="
        };

        var users = new List<User>
        {
            admin
        };

        return users;
    }
}