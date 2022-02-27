using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class CleaningPluginConfiguration : IEntityTypeConfiguration<CleaningPlugin>
	{
		public void Configure(EntityTypeBuilder<CleaningPlugin> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(CleaningPlugin.Id))
				.IsRequired();

			builder.Property(a => a.Name)
				.HasColumnName(nameof(CleaningPlugin.Name))
				.IsRequired();

			builder.Property(a => a.IsActive)
				.HasColumnName(nameof(CleaningPlugin.IsActive))
				.IsRequired();

			builder.Property(a => a.OrdinalNumber)
				.HasColumnName(nameof(CleaningPlugin.OrdinalNumber))
				.IsRequired();

			builder.Property(a => a.Description)
				.HasColumnName(nameof(CleaningPlugin.Description));

			builder.Property(a => a.HotelId)
				.HasColumnName(nameof(CleaningPlugin.HotelId));

			builder.Property(a => a.Data)
				.HasColumnName(nameof(CleaningPlugin.Data))
				.HasColumnType("jsonb")
				.IsRequired();

			builder
				.HasOne(a => a.Hotel)
				.WithMany(h => h.CleaningPlugins)
				.HasForeignKey(a => a.HotelId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
