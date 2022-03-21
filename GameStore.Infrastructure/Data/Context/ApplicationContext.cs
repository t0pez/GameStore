using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.RelationalModels;
using GameStore.Infrastructure.Data.Configurations;
using GameStore.Infrastructure.Data.Configurations.RelationshipConfiguration;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Data.Context;

public sealed class ApplicationContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<PlatformType> PlatformTypes { get; set; }
    public DbSet<Comment> Comments{ get; set; }

    public DbSet<GameGenre> GameGenres { get; set; }
    public DbSet<GamePlatformType> GamePlatformTypes { get; set; }
    
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
        this.SeedData();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CommentConfiguration());
        modelBuilder.ApplyConfiguration(new GameConfiguration());
        modelBuilder.ApplyConfiguration(new GenreConfiguration());
        modelBuilder.ApplyConfiguration(new PlatformTypesConfiguration());
        modelBuilder.ApplyConfiguration(new GameGenreConfiguration());
        modelBuilder.ApplyConfiguration(new GamePlatformConfiguration());
    }
}