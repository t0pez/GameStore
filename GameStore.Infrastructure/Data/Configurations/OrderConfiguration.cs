using GameStore.Core.Models.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(order => order.Id);

        builder.Property(order => order.IsDeleted)
               .IsRequired();
        builder.Property(order => order.OrderDate)
               .IsRequired();
        builder.Property(order => order.CustomerId)
               .IsRequired();

        builder.HasMany(order => order.OrderDetails)
               .WithOne(details => details.Order)
               .HasForeignKey(details => details.OrderId);
    }
}