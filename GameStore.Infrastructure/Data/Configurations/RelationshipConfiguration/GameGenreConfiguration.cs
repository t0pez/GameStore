using GameStore.Core.Models.RelationalModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations.RelationshipConfiguration;

public class GameGenreConfiguration : IEntityTypeConfiguration<GameGenre>
{
    public void Configure(EntityTypeBuilder<GameGenre> builder)
    {
        builder.HasKey(gameGenre => new { gameGenre.GameId, gameGenre.GenreId });
        
        builder.HasOne(gc => gc.Game)
               .WithMany(game => game.Genres)
               .HasForeignKey(gameComment => gameComment.GameId);
        
        builder.HasOne(gc => gc.Genre)
               .WithMany(game => game.Games)
               .HasForeignKey(gameComment => gameComment.GenreId);
    }
}