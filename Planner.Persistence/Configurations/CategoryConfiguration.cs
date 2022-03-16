using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class CategoryConfiguration : IEntityTypeConfiguration<LostAndFoundCategory>
    {
        public void Configure(EntityTypeBuilder<LostAndFoundCategory> builder)
        {
            builder.ConfigureChangeTrackingBaseEntity();

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(LostAndFoundCategory.Id))
                .IsRequired();

            builder.Property(a => a.Name)
                .HasColumnName(nameof(LostAndFoundCategory.Name))
                .IsRequired();

            builder.Property(a => a.ExpirationDays)
                .HasColumnName(nameof(LostAndFoundCategory.ExpirationDays))
                .IsRequired();

        }
    }
}
