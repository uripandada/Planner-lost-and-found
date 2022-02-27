using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Common.Enums;
using Planner.Domain.Entities;
using System;

namespace Planner.Persistence.Configurations
{
	public class CleaningPlanGroupAffinityConfiguration : IEntityTypeConfiguration<CleaningPlanGroupAffinity>
	{
		public void Configure(EntityTypeBuilder<CleaningPlanGroupAffinity> builder)
		{
			builder.HasKey(a => new { a.ReferenceId, a.CleaningPlanGroupId });

			builder.Property(a => a.ReferenceId)
				.HasColumnName(nameof(CleaningPlanGroupAffinity.ReferenceId))
				.IsRequired();

			builder.Property(a => a.AffinityType)
				.HasColumnName(nameof(CleaningPlanGroupAffinity.AffinityType))
				.HasDefaultValue(CleaningPlanGroupAffinityType.UNKNOWN)
				.HasConversion(a => a.ToString(), a => (CleaningPlanGroupAffinityType)Enum.Parse(typeof(CleaningPlanGroupAffinityType), a))
				.IsRequired();

			builder.Property(a => a.CleaningPlanGroupId)
				.HasColumnName(nameof(CleaningPlanGroupAffinity.CleaningPlanGroupId))
				.IsRequired();

			builder
				.HasOne(a => a.CleaningPlanGroup)
				.WithMany(cpg => cpg.Affinities)
				.HasForeignKey(a => a.CleaningPlanGroupId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
