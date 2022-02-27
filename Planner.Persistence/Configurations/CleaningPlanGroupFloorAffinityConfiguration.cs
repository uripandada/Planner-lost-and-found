using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	//public class CleaningPlanGroupFloorAffinityConfiguration : IEntityTypeConfiguration<CleaningPlanGroupFloorAffinity>
	//{
	//	public void Configure(EntityTypeBuilder<CleaningPlanGroupFloorAffinity> builder)
	//	{
	//		builder.HasKey(a => new { a.FloorId, a.CleaningPlanGroupId });

	//		builder.Property(a => a.FloorId)
	//			.HasColumnName(nameof(CleaningPlanGroupFloorAffinity.FloorId))
	//			.IsRequired();

	//		builder.Property(a => a.CleaningPlanGroupId)
	//			.HasColumnName(nameof(CleaningPlanGroupFloorAffinity.CleaningPlanGroupId))
	//			.IsRequired();

	//		builder
	//			.HasOne(a => a.Floor)
	//			.WithMany()
	//			.HasForeignKey(a => a.FloorId)
	//			.OnDelete(DeleteBehavior.Restrict);

	//		builder
	//			.HasOne(a => a.CleaningPlanGroup)
	//			.WithMany(cpg => cpg.FloorAffinities)
	//			.HasForeignKey(a => a.CleaningPlanGroupId)
	//			.OnDelete(DeleteBehavior.Cascade);
	//	}
	//}
}
