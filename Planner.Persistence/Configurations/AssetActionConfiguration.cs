using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class AssetActionConfiguration : IEntityTypeConfiguration<AssetAction>
    {
        public void Configure(EntityTypeBuilder<AssetAction> builder)
        {
            builder.ConfigureChangeTrackingBaseEntity();

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(AssetAction.Id))
                .IsRequired();

            builder.Property(a => a.Name)
                .HasColumnName(nameof(AssetAction.Name))
                .IsRequired();

            builder.Property(a => a.AssetGroupId)
                .HasColumnName(nameof(AssetAction.AssetGroupId))
                .IsRequired();

            builder.Property(a => a.QuickOrTimedKey)
                .HasColumnName(nameof(AssetAction.QuickOrTimedKey));

            builder.Property(a => a.PriorityKey)
                .HasColumnName(nameof(AssetAction.PriorityKey));

            builder.Property(a => a.DefaultAssignedToUserId)
                .HasColumnName(nameof(AssetAction.DefaultAssignedToUserId));

            builder.Property(a => a.DefaultAssignedToUserGroupId)
                .HasColumnName(nameof(AssetAction.DefaultAssignedToUserGroupId));

            builder.Property(a => a.DefaultAssignedToUserSubGroupId)
                .HasColumnName(nameof(AssetAction.DefaultAssignedToUserSubGroupId));

            builder.Property(a => a.Credits)
                .HasColumnName(nameof(AssetAction.Credits));

            builder.Property(a => a.Price)
                .HasColumnName(nameof(AssetAction.Price));

            builder.Property(a => a.IsSystemDefined)
                .HasColumnName(nameof(AssetAction.IsSystemDefined))
                .IsRequired();

            builder.Property(a => a.SystemActionTypeKey)
                .HasColumnName(nameof(AssetAction.SystemActionTypeKey))
                .IsRequired();

            builder
                .HasOne(aa => aa.AssetGroup)
                .WithMany(a => a.AssetActions)
                .HasForeignKey(aa => aa.AssetGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(aa => aa.DefaultAssignedToUser)
                .WithMany()
                .HasForeignKey(aa => aa.DefaultAssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(aa => aa.DefaultAssignedToUserGroup)
                .WithMany()
                .HasForeignKey(aa => aa.DefaultAssignedToUserGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(aa => aa.DefaultAssignedToUserSubGroup)
                .WithMany()
                .HasForeignKey(aa => aa.DefaultAssignedToUserSubGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder
            //    .HasOne(action => action.AssetModel)
            //    .WithMany(model => model.Actions)
            //    .HasForeignKey(action => action.AssetModelId)
            //    .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
