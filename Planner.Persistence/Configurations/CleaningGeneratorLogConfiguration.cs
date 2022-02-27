using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class CleaningGeneratorLogConfiguration : IEntityTypeConfiguration<CleaningGeneratorLog>
	{
		public void Configure(EntityTypeBuilder<CleaningGeneratorLog> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(CleaningGeneratorLog.Id))
				.IsRequired();

			builder.Property(a => a.At)
				.HasColumnName(nameof(CleaningGeneratorLog.At))
				.IsRequired();
			
			builder.Property(a => a.CleaningPlanDate)
				.HasColumnName(nameof(CleaningGeneratorLog.CleaningPlanDate))
				.IsRequired();

			builder.Property(a => a.GenerationId)
				.HasColumnName(nameof(CleaningGeneratorLog.GenerationId))
				.IsRequired();

			builder.Property(a => a.Message)
				.HasColumnName(nameof(CleaningGeneratorLog.Message))
				.IsRequired();

			builder.Property(a => a.HotelId)
				.HasColumnName(nameof(CleaningGeneratorLog.HotelId))
				.IsRequired();

			builder.Property(a => a.CleaningEventsDescription).HasColumnName(nameof(CleaningGeneratorLog.CleaningEventsDescription));
			builder.Property(a => a.CleaningsDescription).HasColumnName(nameof(CleaningGeneratorLog.CleaningsDescription));
			builder.Property(a => a.OrderedPluginsDescription).HasColumnName(nameof(CleaningGeneratorLog.OrderedPluginsDescription));
			builder.Property(a => a.PluginEventsDescription).HasColumnName(nameof(CleaningGeneratorLog.PluginEventsDescription));
			builder.Property(a => a.ReservationsDescription).HasColumnName(nameof(CleaningGeneratorLog.ReservationsDescription));
			builder.Property(a => a.ReservationsEventsDescription).HasColumnName(nameof(CleaningGeneratorLog.ReservationsEventsDescription));
			builder.Property(a => a.RoomDescription).HasColumnName(nameof(CleaningGeneratorLog.RoomDescription));
		}
	}
}
