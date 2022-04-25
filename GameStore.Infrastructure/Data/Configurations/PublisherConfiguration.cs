using GameStore.Core.Models.Publishers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations;

internal class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
        builder.HasKey(publisher => publisher.Id);

        builder.Property(publisher => publisher.Name).IsRequired();
        builder.Property(publisher => publisher.Description).IsRequired();
        builder.Property(publisher => publisher.HomePage).IsRequired();

        builder.HasMany(publisher => publisher.Games)
               .WithOne(game => game.Publisher)
               .HasForeignKey(game => game.PublisherId);
    }
}