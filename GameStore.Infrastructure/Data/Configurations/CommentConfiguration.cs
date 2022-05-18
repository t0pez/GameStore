using GameStore.Core.Models.Comments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations;

internal class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(comment => comment.Id);

        builder.Property(comment => comment.Name).IsRequired();
        builder.Property(comment => comment.Body).IsRequired();
        builder.Property(comment => comment.DateOfCreation).IsRequired();
        builder.Property(comment => comment.IsDeleted).IsRequired();
        builder.Property(comment => comment.State).IsRequired();

        builder.HasOne(comment => comment.Game)
               .WithMany(game => game.Comments)
               .HasForeignKey(comment => comment.GameId);
        builder.HasOne(comment => comment.Parent)
               .WithMany(comment => comment.Replies)
               .HasForeignKey(reply => reply.ParentId);
        builder.HasMany(comment => comment.Replies)
               .WithOne(reply => reply.Parent)
               .HasForeignKey(reply => reply.ParentId);
    }
}