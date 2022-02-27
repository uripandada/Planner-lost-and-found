using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class RccProductConfiguration : IEntityTypeConfiguration<RccProduct>
    {
        public void Configure(EntityTypeBuilder<RccProduct> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(RccProduct.Id))
                .IsRequired();

            builder.Property(a => a.IsActive)
                .HasColumnName(nameof(RccProduct.IsActive))
                .IsRequired();

            builder.Property(a => a.ExternalName)
                .HasColumnName(nameof(RccProduct.ExternalName))
                .IsRequired();
            
            builder.Property(a => a.ServiceId)
                .HasColumnName(nameof(RccProduct.ServiceId));
            
            builder.Property(a => a.CategoryId)
                .HasColumnName(nameof(RccProduct.CategoryId));
        }
    }
}
