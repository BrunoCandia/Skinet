using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Configurations
{
    public class DeliveryMethodConfiguration : IEntityTypeConfiguration<DeliveryMethod>
    {
        public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(p => p.ShortName).HasMaxLength(256);
            builder.Property(p => p.Description).HasMaxLength(256);
            builder.Property(p => p.DeliveryTime).HasMaxLength(256);
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
        }
    }
}
