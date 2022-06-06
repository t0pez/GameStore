using GameStore.Core.Models.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations;

public class OpenedOrderConfiguration : IEntityTypeConfiguration<OpenedOrder>
{
    public void Configure(EntityTypeBuilder<OpenedOrder> builder)
    {
        builder.HasKey(order => order.OrderId);

        builder.HasOne<Order>()
               .WithMany()
               .HasForeignKey(openedOrder => openedOrder.OrderId);
    }
}