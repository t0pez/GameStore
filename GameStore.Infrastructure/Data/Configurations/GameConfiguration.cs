using GameStore.Core.Models.Games;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(game => game.Id);

        builder.HasAlternateKey(game => game.Key);
        builder.Property(game => game.Name).IsRequired();
        builder.Property(game => game.Description).IsRequired();
        builder.Property(game => game.IsDeleted).IsRequired();

        builder.HasMany(game => game.Comments)
            .WithOne(comment => comment.Game)
            .HasForeignKey(comment => comment.GameId);
        builder.HasMany(game => game.Genres)
            .WithMany(genre => genre.Games);
        builder.HasMany(game => game.PlatformTypes)
            .WithMany(platformType => platformType.Games);
    }
}