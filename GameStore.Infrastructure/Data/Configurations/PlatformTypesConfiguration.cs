using GameStore.Core.Models.Games;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations;

internal class PlatformTypesConfiguration : IEntityTypeConfiguration<PlatformType>
{
    public void Configure(EntityTypeBuilder<PlatformType> builder)
    {
        builder.HasKey(pt => pt.Id);

        builder.Property(pt => pt.Name).IsRequired();
        builder.Property(pt => pt.IsDeleted).IsRequired();

        builder.HasMany(pt => pt.Games).WithMany(g => g.PlatformTypes);
    }
}