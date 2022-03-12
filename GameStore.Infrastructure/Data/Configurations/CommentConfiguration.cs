using GameStore.Core.Models.Comments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations;

internal class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired();
        builder.Property(c => c.Body).IsRequired();
        builder.Property(c => c.DateOfCreation).IsRequired();
        builder.Property(c => c.IsDeleted).IsRequired();

        builder.HasOne(c => c.Game).WithMany(g => g.Comments).HasForeignKey(c => c.GameId);
        builder.HasOne(c => c.Parent).WithMany(c => c.Replies).HasForeignKey(c => c.ParentId);
        builder.HasMany(c => c.Replies).WithOne(c => c.Parent);
    }
}