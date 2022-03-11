using GameStore.Core.Models.Games;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations;

internal class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name).IsRequired();
        builder.Property(g => g.IsDeleted).IsRequired();

        builder.HasMany(g => g.SubGenres);
        builder.HasMany(g => g.Games).WithMany(g => g.Genres);
    }
}