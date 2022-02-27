using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class SystemTaskHistoryConfiguration : IEntityTypeConfiguration<SystemTaskHistory>
	{
		public void Configure(EntityTypeBuilder<SystemTaskHistory> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(SystemTaskHistory.Id))
				.IsRequired();

			builder
				.Property(a => a.ChangedByKey)
				.HasColumnName(nameof(SystemTaskHistory.ChangedByKey))
				.IsRequired();

			builder
				.Property(a => a.CreatedAt)
				.HasColumnName(nameof(SystemTaskHistory.CreatedAt))
				.IsRequired();

			builder
				.Property(a => a.CreatedById)
				.HasColumnName(nameof(SystemTaskHistory.CreatedById));

			builder
				.Property(a => a.Message)
				.HasColumnName(nameof(SystemTaskHistory.Message))
				.IsRequired();

			builder
				.Property(a => a.NewData)
				.HasColumnName(nameof(SystemTaskHistory.NewData))
				.HasColumnType("jsonb")
				.IsRequired();

			builder
				.Property(a => a.OldData)
				.HasColumnName(nameof(SystemTaskHistory.OldData))
				.HasColumnType("jsonb")
				.IsRequired();

			builder
				.Property(a => a.SystemTaskId)
				.HasColumnName(nameof(SystemTaskHistory.SystemTaskId))
				.IsRequired();

			builder
				.HasOne(a => a.CreatedBy)
				.WithMany()
				.HasForeignKey(a => a.CreatedById)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.SystemTask)
				.WithMany(t => t.History)
				.HasForeignKey(a => a.SystemTaskId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
