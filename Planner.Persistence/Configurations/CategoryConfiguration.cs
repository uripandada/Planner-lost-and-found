using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ConfigureChangeTrackingBaseEntity();

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(Category.Id))
                .IsRequired();

            builder.Property(a => a.Name)
                .HasColumnName(nameof(Category.Name))
                .IsRequired();

            builder.Property(a => a.ExpirationDays)
                .HasColumnName(nameof(Category.ExpirationDays))
                .IsRequired();

        }
    }
}
