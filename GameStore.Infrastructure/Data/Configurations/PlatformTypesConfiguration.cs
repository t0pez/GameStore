using GameStore.Core.Models.Games;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations;

internal class PlatformTypesConfiguration : IEntityTypeConfiguration<PlatformType>
{
    public void Configure(EntityTypeBuilder<PlatformType> builder)
    {
        builder.HasKey(platformType => platformType.Id);

        builder.Property(platformType => platformType.Name).IsRequired();
        builder.Property(platformType => platformType.IsDeleted).IsRequired();
    }
}