using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class AssetGroupConfiguration : IEntityTypeConfiguration<AssetGroup>
    {
        public void Configure(EntityTypeBuilder<AssetGroup> builder)
        {
            builder.ConfigureChangeTrackingBaseEntity();

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(AssetGroup.Id))
                .IsRequired();

            builder.Property(a => a.Name)
                .HasColumnName(nameof(AssetGroup.Name))
                .IsRequired();

            builder.Property(a => a.TypeKey)
                .HasColumnName(nameof(AssetGroup.TypeKey))
                .IsRequired();

            builder
                .HasMany(g => g.GroupAssets)
                .WithOne(a => a.AssetGroup)
                .HasForeignKey(a => a.AssetGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(g => g.SubGroupAssets)
                .WithOne(a => a.AssetSubGroup)
                .HasForeignKey(a => a.AssetSubGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(g => g.AssetActions)
                .WithOne(a => a.AssetGroup)
                .HasForeignKey(a => a.AssetGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(g => g.ParentAssetGroup)
                .WithMany(g => g.ChildAssetGroups)
                .HasForeignKey(g => g.ParentAssetGroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
