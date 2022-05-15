using GameStore.Core.Models.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations;

public class OrderDetailsConfiguration : IEntityTypeConfiguration<OrderDetails>
{
    public void Configure(EntityTypeBuilder<OrderDetails> builder)
    {
        builder.HasKey(details => details.Id);
        
        builder.Property(details => details.Quantity)
               .IsRequired();
        builder.Property(details => details.Discount)
               .IsRequired();
        builder.Property(details => details.Price)
               .IsRequired();
    }
}