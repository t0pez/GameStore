using GameStore.Core.Models.Server.Games;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations;

internal class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(game => game.Id);
        builder.HasIndex(game => game.Key).IsUnique();

        builder.Property(game => game.Name).IsRequired();
        builder.Property(game => game.Description).IsRequired();
        builder.Property(game => game.File).IsRequired();
        builder.Property(game => game.IsDeleted).IsRequired();

        builder.HasMany(game => game.Comments)
               .WithOne(comment => comment.Game)
               .HasForeignKey(comment => comment.GameId);
    }
}