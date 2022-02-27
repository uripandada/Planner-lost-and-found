using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class SystemTaskActionConfiguration : IEntityTypeConfiguration<SystemTaskAction>
    {
        public void Configure(EntityTypeBuilder<SystemTaskAction> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(SystemTaskAction.Id))
                .IsRequired();

            builder
                .Property(a => a.ActionName)
				.HasColumnName(nameof(SystemTaskAction.ActionName))
				.IsRequired();

			builder
				.Property(a => a.AssetId)
				.HasColumnName(nameof(SystemTaskAction.AssetId))
				.IsRequired();

			builder
				.Property(a => a.AssetGroupId)
				.HasColumnName(nameof(SystemTaskAction.AssetGroupId))
                .IsRequired();

            builder
                .Property(a => a.AssetGroupName)
                .HasColumnName(nameof(SystemTaskAction.AssetGroupName))
                .IsRequired();

            builder
                .Property(a => a.AssetName)
                .HasColumnName(nameof(SystemTaskAction.AssetName))
                .IsRequired();

            builder
				.Property(a => a.AssetQuantity)
				.HasColumnName(nameof(SystemTaskAction.AssetQuantity))
				.HasDefaultValue(1)
				.IsRequired();

            builder
                .Property(a => a.SystemTaskId)
                .HasColumnName(nameof(SystemTaskAction.SystemTaskId))
                .IsRequired();

            builder
                .HasOne(t => t.SystemTask)
                .WithMany(c => c.Actions)
                .HasForeignKey(t => t.SystemTaskId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
