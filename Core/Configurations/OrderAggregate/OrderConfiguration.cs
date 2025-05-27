using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Configurations.OrderAggregate
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(p => p.Subtotal).HasColumnType("decimal(18,2)");
            builder.Property(p => p.BuyerEmail).HasMaxLength(256);
            builder.Property(p => p.PaymentIntentId).HasMaxLength(256);

            // Owned entities
            builder.OwnsOne(p => p.ShippingAddress, o =>
            {
                o.WithOwner();
                o.Property(x => x.Name).HasMaxLength(256);
                o.Property(x => x.Line1).HasMaxLength(256);
                o.Property(x => x.Line2).HasMaxLength(256);
                o.Property(x => x.City).HasMaxLength(256);
                o.Property(x => x.State).HasMaxLength(256);
                o.Property(x => x.PostalCode).HasMaxLength(256);
                o.Property(x => x.Country).HasMaxLength(256);
            });

            builder.OwnsOne(p => p.PaymentSummary, o =>
            {
                o.WithOwner();
                o.Property(x => x.Brand).HasMaxLength(256);
            });

            ////builder.OwnsOne(p => p.ShippingAddress, o => o.WithOwner());
            ////builder.OwnsOne(p => p.PaymentSummary, o => o.WithOwner());

            // Conversions
            builder.Property(p => p.Status)
                   .HasMaxLength(50)
                   .HasConversion(status => status.ToString(),
                                  status => Enum.Parse<OrderStatus>(status));

            ////builder.Property(p => p.Status)
            ////       .HasConversion(status => status.ToString(),
            ////                      status => (OrderStatus)Enum.Parse(typeof(OrderStatus), status));            

            // Navigation properties

            // Implicit Foreign Key Configuration. EF Core will automatically create a foreign key for the OrderItems collection.
            builder.HasMany(p => p.OrderItems)
                   .WithOne()
                   .OnDelete(DeleteBehavior.Cascade);

            // Explicit Foreign Key Configuration.
            ////builder.HasMany(p => p.OrderItems)
            ////       .WithOne(p => p.Order)
            ////       .HasForeignKey(p => p.OrderId)
            ////       .OnDelete(DeleteBehavior.Cascade);

            // This required to add the foreign key property to the OrderItem entity:
            ////// Foreign key property
            ////public Guid OrderId { get; set; }
            ////public Order Order { get; set; } = null!;    
        }
    }
}