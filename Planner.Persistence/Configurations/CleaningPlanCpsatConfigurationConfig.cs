using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class CleaningPlanCpsatConfigurationConfig : IEntityTypeConfiguration<CleaningPlanCpsatConfiguration>
	{
		public void Configure(EntityTypeBuilder<CleaningPlanCpsatConfiguration> builder)
		{
			builder.HasKey(c => c.Id);

			builder.Property(c => c.Id)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.Id))
				.IsRequired();

			builder.Property(c => c.PlanningStrategyTypeKey)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.PlanningStrategyTypeKey))
				.IsRequired();

			builder.Property(c => c.BalanceByRoomsMinRooms)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.BalanceByRoomsMinRooms))
				.IsRequired();

			builder.Property(c => c.BalanceByRoomsMaxRooms)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.BalanceByRoomsMaxRooms))
				.IsRequired();

			builder.Property(c => c.BalanceByCreditsStrictMinCredits)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.BalanceByCreditsStrictMinCredits))
				.IsRequired();

			builder.Property(c => c.BalanceByCreditsStrictMaxCredits)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.BalanceByCreditsStrictMaxCredits))
				.IsRequired();

			builder.Property(c => c.BalanceByCreditsWithAffinitiesMinCredits)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.BalanceByCreditsWithAffinitiesMinCredits))
				.IsRequired();

			builder.Property(c => c.BalanceByCreditsWithAffinitiesMaxCredits)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.BalanceByCreditsWithAffinitiesMaxCredits))
				.IsRequired();

			builder.Property(c => c.TargetByRoomsValue)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.TargetByRoomsValue));

			builder.Property(c => c.TargetByCreditsValue)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.TargetByCreditsValue));

			builder.Property(c => c.DoBalanceStaysAndDepartures)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.DoBalanceStaysAndDepartures))
				.IsRequired();

			builder.Property(c => c.WeightEpsilonStayDeparture)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.WeightEpsilonStayDeparture))
				.IsRequired();

			builder.Property(c => c.MaxStay)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.MaxStay))
				.IsRequired();

			builder.Property(c => c.MaxDeparture)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.MaxDeparture))
				.IsRequired();

			builder.Property(c => c.MaxTravelTime)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.MaxTravelTime))
				.IsRequired();

			builder.Property(c => c.MaxBuildingTravelTime)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.MaxBuildingTravelTime))
				.IsRequired();

			builder.Property(c => c.MaxNumberOfBuildingsPerAttendant)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.MaxNumberOfBuildingsPerAttendant))
				.IsRequired();

			builder.Property(c => c.MaxNumberOfLevelsPerAttendant)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.MaxNumberOfLevelsPerAttendant))
				.IsRequired();

			builder.Property(c => c.RoomAward)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.RoomAward))
				.IsRequired();

			builder.Property(c => c.LevelAward)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.LevelAward))
				.IsRequired();

			builder.Property(c => c.BuildingAward)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.BuildingAward))
				.IsRequired();

			builder.Property(c => c.WeightTravelTime)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.WeightTravelTime))
				.HasDefaultValue(0)
				.IsRequired();

			builder.Property(c => c.WeightCredits)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.WeightCredits))
				.HasDefaultValue(0)
				.IsRequired();
			
			builder.Property(c => c.WeightRoomsCleaned)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.WeightRoomsCleaned))
				.HasDefaultValue(0)
				.IsRequired();
			
			builder.Property(c => c.WeightLevelChange)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.WeightLevelChange))
				.HasDefaultValue(-1)
				.IsRequired();

			builder.Property(c => c.SolverRunTime)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.SolverRunTime))
				.IsRequired();

			builder.Property(c => c.DoesLevelMovementReduceCredits)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.DoesLevelMovementReduceCredits))
				.IsRequired();

			builder.Property(c => c.ApplyLevelMovementCreditReductionAfterNumberOfLevels)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.ApplyLevelMovementCreditReductionAfterNumberOfLevels))
				.IsRequired();

			builder.Property(c => c.LevelMovementCreditsReduction)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.LevelMovementCreditsReduction))
				.IsRequired();

			builder.Property(c => c.DoUsePrePlan)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.DoUsePrePlan))
				.IsRequired();

			builder.Property(c => c.DoUsePreAffinity)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.DoUsePreAffinity))
				.IsRequired();

			builder.Property(c => c.DoCompleteProposedPlanOnUsePreplan)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.DoCompleteProposedPlanOnUsePreplan))
				.IsRequired();

			builder.Property(c => c.DoesBuildingMovementReduceCredits)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.DoesBuildingMovementReduceCredits))
				.IsRequired();

			builder.Property(c => c.BuildingMovementCreditsReduction)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.BuildingMovementCreditsReduction))
				.IsRequired();

			builder.Property(c => c.ArePreferredLevelsExclusive)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.ArePreferredLevelsExclusive))
				.IsRequired();

			builder.Property(c => c.CleaningPriorityKey)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.CleaningPriorityKey))
				.IsRequired();

			builder.Property(c => c.BuildingsDistanceMatrix)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.BuildingsDistanceMatrix));

			builder.Property(c => c.LevelsDistanceMatrix)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.LevelsDistanceMatrix));
			
			builder.Property(c => c.LimitAttendantsPerLevel)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.LimitAttendantsPerLevel));
			
			builder.Property(c => c.MaxNumberOfBuildingsPerAttendant)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.MaxNumberOfBuildingsPerAttendant))
				.HasDefaultValue(0)
				.IsRequired();
			
			builder.Property(c => c.MaxNumberOfLevelsPerAttendant)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.MaxNumberOfLevelsPerAttendant))
				.HasDefaultValue(0)
				.IsRequired();

			builder.Property(a => a.MinutesPerCredit)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.MinutesPerCredit))
				.IsRequired();

			builder.Property(a => a.MinCreditsForMultipleCleanersCleaning)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.MinCreditsForMultipleCleanersCleaning))
				.IsRequired();

			builder.Property(a => a.MaxDeparturesReducesCredits)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.MaxDeparturesReducesCredits))
				.IsRequired();

			builder.Property(a => a.MaxDeparturesEquivalentCredits)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.MaxDeparturesEquivalentCredits))
				.IsRequired();

			builder.Property(a => a.MaxDeparturesReductionThreshold)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.MaxDeparturesReductionThreshold))
				.IsRequired();

			builder.Property(a => a.MaxStaysIncreasesCredits)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.MaxStaysIncreasesCredits))
				.IsRequired();

			builder.Property(a => a.MaxStaysEquivalentCredits)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.MaxStaysEquivalentCredits))
				.IsRequired();

			builder.Property(a => a.MaxStaysIncreaseThreshold)
				.HasColumnName(nameof(CleaningPlanCpsatConfiguration.MaxStaysIncreaseThreshold))
				.IsRequired();

			builder
				.HasOne(c => c.CleaningPlan)
				.WithOne(cp => cp.CleaningPlanCpsatConfiguration)
				.HasForeignKey<CleaningPlanCpsatConfiguration>(c => c.Id)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
