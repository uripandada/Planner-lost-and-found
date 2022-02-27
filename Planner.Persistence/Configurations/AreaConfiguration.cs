using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class AreaConfiguration : IEntityTypeConfiguration<Area>
    {
        public void Configure(EntityTypeBuilder<Area> builder)
        {
            builder.ConfigureBaseEntity();

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(Area.Id))
                .IsRequired();

            builder.Property(a => a.Name)
                .HasColumnName(nameof(Area.Name))
                .IsRequired();

            builder
                .HasMany(a => a.Buildings)
                .WithOne(b => b.Area)
                .HasForeignKey(b => b.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(a => a.Rooms)
                .WithOne(r => r.Area)
                .HasForeignKey(r => r.AreaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
