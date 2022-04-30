using GameStore.Core.Models.Genres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations;

internal class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.HasKey(genre => genre.Id);

        builder.Property(genre => genre.Name).IsRequired();
        builder.Property(genre => genre.IsDeleted).IsRequired();

        builder.HasMany(genre => genre.SubGenres)
               .WithOne(subGenre => subGenre.Parent)
               .HasForeignKey(subGenre => subGenre.ParentId);
    }
}