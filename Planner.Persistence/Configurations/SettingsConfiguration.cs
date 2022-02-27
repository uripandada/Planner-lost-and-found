using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class SettingsConfiguration : IEntityTypeConfiguration<Settings>
	{
		public void Configure(EntityTypeBuilder<Settings> builder)
		{
			builder.ConfigureChangeTrackingBaseEntity();

			builder.HasKey(s => s.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(Settings.Id))
				.IsRequired();

			builder.Property(a => a.AllowPostponeCleanings)
				.HasColumnName(nameof(Settings.AllowPostponeCleanings))
				.IsRequired();

			builder.Property(a => a.DefaultAttendantEndTime)
				.HasColumnName(nameof(Settings.DefaultAttendantEndTime))
				.IsRequired();

			builder.Property(a => a.DefaultAttendantMaxCredits)
				.HasColumnName(nameof(Settings.DefaultAttendantMaxCredits));

			builder.Property(a => a.DefaultAttendantStartTime)
				.HasColumnName(nameof(Settings.DefaultAttendantStartTime))
				.IsRequired();

			builder.Property(a => a.DefaultCheckInTime)
				.HasColumnName(nameof(Settings.DefaultCheckInTime))
				.IsRequired();

			builder.Property(a => a.DefaultCheckOutTime)
				.HasColumnName(nameof(Settings.DefaultCheckOutTime))
				.IsRequired();

			builder.Property(a => a.EmailAddressesForSendingPlan)
				.HasColumnName(nameof(Settings.EmailAddressesForSendingPlan));

			builder.Property(a => a.FromEmailAddress)
				.HasColumnName(nameof(Settings.FromEmailAddress));

			builder.Property(a => a.ReserveBetweenCleanings)
				.HasColumnName(nameof(Settings.ReserveBetweenCleanings));

			builder.Property(a => a.SendPlanToAttendantsByEmail)
				.HasColumnName(nameof(Settings.SendPlanToAttendantsByEmail))
				.IsRequired();

			builder.Property(a => a.ShowCleaningDelays)
				.HasColumnName(nameof(Settings.ShowCleaningDelays))
				.IsRequired();

			builder.Property(a => a.ShowHoursInWorkerPlanner)
				.HasColumnName(nameof(Settings.ShowHoursInWorkerPlanner))
				.IsRequired();

			builder.Property(a => a.TravelReserve)
				.HasColumnName(nameof(Settings.TravelReserve));

			builder.Property(a => a.UseGroups)
				.HasColumnName(nameof(Settings.UseGroups))
				.IsRequired();

			builder.Property(a => a.CleanHostelRoomBedsInGroups)
				.HasColumnName(nameof(Settings.CleanHostelRoomBedsInGroups))
				.IsRequired();

			builder.Property(a => a.UseOrderInPlanning)
				.HasColumnName(nameof(Settings.UseOrderInPlanning))
				.IsRequired();

			builder.Property(a => a.BuildingsDistanceMatrix)
				.HasColumnName(nameof(Settings.BuildingsDistanceMatrix));

			builder.Property(a => a.LevelsDistanceMatrix)
				.HasColumnName(nameof(Settings.LevelsDistanceMatrix));


			builder.Property(a => a.BuildingAward)
				.HasColumnName(nameof(Settings.BuildingAward))
				.HasDefaultValue(0)
				.IsRequired();

			builder.Property(a => a.LevelAward)
				.HasColumnName(nameof(Settings.LevelAward))
				.HasDefaultValue(0)
				.IsRequired();

			builder.Property(a => a.RoomAward)
				.HasColumnName(nameof(Settings.RoomAward))
				.HasDefaultValue(0)
				.IsRequired();

			builder.Property(a => a.LevelTime)
				.HasColumnName(nameof(Settings.LevelTime))
				.HasDefaultValue(0)
				.IsRequired();

			builder.Property(a => a.CleaningTime)
				.HasColumnName(nameof(Settings.CleaningTime))
				.HasDefaultValue(0)
				.IsRequired();
			
			builder.Property(a => a.WeightLevelChange)
				.HasColumnName(nameof(Settings.WeightLevelChange))
				.IsRequired();
			
			builder.Property(a => a.WeightCredits)
				.HasColumnName(nameof(Settings.WeightCredits))
				.IsRequired();
			
			builder.Property(a => a.MinutesPerCredit)
				.HasColumnName(nameof(Settings.MinutesPerCredit))
				.IsRequired();
			
			builder.Property(a => a.MinCreditsForMultipleCleanersCleaning)
				.HasColumnName(nameof(Settings.MinCreditsForMultipleCleanersCleaning))
				.IsRequired();

			builder
				.HasOne(s => s.Hotel)
				.WithOne(h => h.Settings)
				.HasForeignKey<Settings>(s => s.HotelId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
