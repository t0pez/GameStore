using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Games;
using GameStore.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Data.Context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<PlatformType> PlatformTypes { get; set; }
        public DbSet<Comment> Comments{ get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new GameConfiguration());
            modelBuilder.ApplyConfiguration(new GenreConfiguration());
            modelBuilder.ApplyConfiguration(new PlatformTypesConfiguration());
        }
    }
}
