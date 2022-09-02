using GameStore.Core.Models.Server.Comments;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.Genres;
using GameStore.Core.Models.Server.Orders;
using GameStore.Core.Models.Server.PlatformTypes;
using GameStore.Core.Models.Server.Publishers;
using GameStore.Core.Models.Server.RelationalModels;
using GameStore.Core.Models.Server.Users;
using GameStore.Infrastructure.Data.Context.DataSeed;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Data.Context;

public sealed class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    public DbSet<Game> Games { get; set; }

    public DbSet<Genre> Genres { get; set; }

    public DbSet<PlatformType> PlatformTypes { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<Publisher> Publishers { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderDetails> OrderDetails { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<GameGenre> GameGenres { get; set; }

    public DbSet<GamePlatformType> GamePlatformTypes { get; set; }

    public DbSet<OpenedOrder> OpenedOrders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        modelBuilder.SeedData();
    }
}