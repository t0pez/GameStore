using GameStore.Core.Models.Games;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(g => g.Id);

        builder.HasAlternateKey(g => g.Key);
        builder.Property(g => g.Name).IsRequired();
        builder.Property(g => g.Description).IsRequired();
        builder.Property(g => g.IsDeleted).IsRequired();

        builder.HasMany(g => g.Comments);
        builder.HasMany(g => g.Genres)
            .WithMany(g => g.Games);
        builder.HasMany(g => g.PlatformTypes)
            .WithMany(pt => pt.Games);
    }
}