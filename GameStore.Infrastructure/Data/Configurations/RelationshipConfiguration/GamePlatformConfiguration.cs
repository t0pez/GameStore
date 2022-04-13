using GameStore.Core.Models.RelationalModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations.RelationshipConfiguration;

internal class GamePlatformConfiguration : IEntityTypeConfiguration<GamePlatformType>
{
    public void Configure(EntityTypeBuilder<GamePlatformType> builder)
    {
        builder.HasKey(gamePlatform => new { gamePlatform.GameId, gamePlatform.PlatformId });

        builder.HasOne(gamePlatform => gamePlatform.Game)
               .WithMany(game => game.Platforms)
               .HasForeignKey(platform => platform.GameId);

        builder.HasOne(gamePlatform => gamePlatform.Platform)
               .WithMany(game => game.Games)
               .HasForeignKey(platform => platform.PlatformId);
    }
}