using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Configurations.OrderAggregate
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");

            // Owned entities
            builder.OwnsOne(p => p.ItemOrdered, o =>
            {
                o.WithOwner();
                o.Property(x => x.ProductName).HasMaxLength(256);
                o.Property(x => x.PictureUrl).HasMaxLength(256);
            });
        }
    }
}
