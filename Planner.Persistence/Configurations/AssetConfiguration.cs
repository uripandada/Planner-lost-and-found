using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class AssetConfiguration : IEntityTypeConfiguration<Asset>
    {
        public void Configure(EntityTypeBuilder<Asset> builder)
        {
            builder.ConfigureChangeTrackingBaseEntity();

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(Asset.Id))
                .IsRequired();

            builder.Property(a => a.Name)
                .HasColumnName(nameof(Asset.Name))
                .IsRequired();

            builder.Property(a => a.SerialNumber)
                .HasColumnName(nameof(Asset.SerialNumber));

            builder.Property(a => a.AssetGroupId)
                .HasColumnName(nameof(Asset.AssetGroupId));

            builder.Property(a => a.AssetSubGroupId)
                .HasColumnName(nameof(Asset.AssetSubGroupId));

            builder.Property(a => a.IsBulk)
                .HasColumnName(nameof(Asset.IsBulk))
                .IsRequired();

            builder
                .HasMany(a => a.AssetTags)
                .WithOne(at => at.Asset)
                .HasForeignKey(at => at.AssetId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(a => a.AssetFiles)
                .WithOne(af => af.Asset)
                .HasForeignKey(af => af.AssetId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder
            //    .HasMany(a => a.Actions)
            //    .WithOne(aa => aa.Asset)
            //    .HasForeignKey(aa => aa.AssetId)
            //    .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(a => a.AssetGroup)
                .WithMany(g => g.GroupAssets)
                .HasForeignKey(a => a.AssetGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(a => a.AssetSubGroup)
                .WithMany(g => g.SubGroupAssets)
                .HasForeignKey(a => a.AssetSubGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(a => a.WarehouseAvailabilities)
                .WithOne(aa => aa.Asset)
                .HasForeignKey(aa => aa.AssetId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(a => a.RoomUsages)
                .WithOne(aa => aa.Asset)
                .HasForeignKey(aa => aa.AssetId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(a => a.InventoryStatuses)
                .WithOne(aa => aa.Asset)
                .HasForeignKey(aa => aa.AssetId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(a => a.WarehouseDocuments)
                .WithOne(aa => aa.Asset)
                .HasForeignKey(aa => aa.AssetId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(a => a.WarehouseDocumentArchives)
                .WithOne(aa => aa.Asset)
                .HasForeignKey(aa => aa.AssetId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
