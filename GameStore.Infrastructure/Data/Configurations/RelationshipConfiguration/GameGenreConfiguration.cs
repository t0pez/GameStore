using GameStore.Core.Models.Server.RelationalModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations.RelationshipConfiguration;

internal class GameGenreConfiguration : IEntityTypeConfiguration<GameGenre>
{
    public void Configure(EntityTypeBuilder<GameGenre> builder)
    {
        builder.HasKey(gameGenre => new { gameGenre.GameId, gameGenre.GenreId });

        builder.HasOne(gameComment => gameComment.Game)
               .WithMany(game => game.Genres)
               .HasForeignKey(gameComment => gameComment.GameId);

        builder.HasOne(gameComment => gameComment.Genre)
               .WithMany(game => game.Games)
               .HasForeignKey(gameComment => gameComment.GenreId);
    }
}