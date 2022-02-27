using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;
using System;

namespace Planner.Persistence.Configurations
{
	public class CleaningInspectionConfiguration : IEntityTypeConfiguration<CleaningInspection>
	{
		public void Configure(EntityTypeBuilder<CleaningInspection> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(CleaningInspection.Id))
				.IsRequired();

			builder.Property(a => a.CreatedById)
				.HasColumnName(nameof(CleaningInspection.CreatedById))
				.IsRequired();

			builder
				.HasOne(a => a.CreatedBy)
				.WithMany()
				.HasForeignKey(a => a.CreatedById)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(a => a.CleaningId)
				.HasColumnName(nameof(CleaningInspection.CleaningId))
				.IsRequired();

			builder
				.HasOne(h => h.Cleaning)
				.WithMany(i => i.CleaningInspections)
				.HasForeignKey(h => h.CleaningId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(a => a.StartedAt)
				.HasColumnName(nameof(CleaningInspection.StartedAt))
				.IsRequired();

			builder.Property(a => a.EndedAt)
				.HasColumnName(nameof(CleaningInspection.EndedAt));

			builder.Property(a => a.IsFinished)
				.HasColumnName(nameof(CleaningInspection.IsFinished))
				.IsRequired();

			builder.Property(a => a.IsSuccess)
				.HasColumnName(nameof(CleaningInspection.IsSuccess))
				.IsRequired();

			builder.Property(a => a.Note)
				.HasColumnName(nameof(CleaningInspection.Note));
		}
	}
}
