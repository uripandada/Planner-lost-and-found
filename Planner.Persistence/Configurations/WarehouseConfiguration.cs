using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
	{
		public void Configure(EntityTypeBuilder<Warehouse> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(Warehouse.Id))
				.IsRequired();

			builder.Property(a => a.Name)
				.HasColumnName(nameof(Warehouse.Name))
				.IsRequired();

			builder.Property(a => a.IsCentral)
				.HasColumnName(nameof(Warehouse.IsCentral))
				.IsRequired();

			builder.Property(a => a.FloorId)
				.HasColumnName(nameof(Warehouse.FloorId));

			builder.Property(a => a.HotelId)
				.HasColumnName(nameof(Warehouse.HotelId));

			builder
				.HasOne(a => a.Hotel)
				.WithMany()
				.HasForeignKey(a => a.HotelId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.Floor)
				.WithMany(f => f.Warehouses)
				.HasForeignKey(a => a.FloorId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(w => w.AssetAvailabilities)
				.WithOne(aa => aa.Warehouse)
				.HasForeignKey(aa => aa.WarehouseId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(w => w.Inventories)
				.WithOne(aa => aa.Warehouse)
				.HasForeignKey(aa => aa.WarehouseId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(w => w.WarehouseDocuments)
				.WithOne(aa => aa.Warehouse)
				.HasForeignKey(aa => aa.WarehouseId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(w => w.WarehouseDocumentArchives)
				.WithOne(aa => aa.Warehouse)
				.HasForeignKey(aa => aa.WarehouseId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}

	public class WarehouseAssetAvailabilityConfiguration : IEntityTypeConfiguration<WarehouseAssetAvailability>
	{
		public void Configure(EntityTypeBuilder<WarehouseAssetAvailability> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(WarehouseAssetAvailability.Id))
				.IsRequired();

			builder.Property(a => a.Quantity)
				.HasColumnName(nameof(WarehouseAssetAvailability.Quantity))
				.IsRequired();

			builder.Property(a => a.ReservedQuantity)
				.HasColumnName(nameof(WarehouseAssetAvailability.ReservedQuantity))
				.IsRequired();

			builder.Property(a => a.WarehouseId)
				.HasColumnName(nameof(WarehouseAssetAvailability.WarehouseId))
				.IsRequired();

			builder.Property(a => a.AssetId)
				.HasColumnName(nameof(WarehouseAssetAvailability.AssetId))
				.IsRequired();

			builder.UseXminAsConcurrencyToken();

			builder
				.HasOne(a => a.Warehouse)
				.WithMany(w => w.AssetAvailabilities)
				.HasForeignKey(a => a.WarehouseId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.Asset)
				.WithMany(aa => aa.WarehouseAvailabilities)
				.HasForeignKey(a => a.AssetId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}

	public class RoomAssetUsageConfiguration : IEntityTypeConfiguration<RoomAssetUsage>
	{
		public void Configure(EntityTypeBuilder<RoomAssetUsage> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(RoomAssetUsage.Id))
				.IsRequired();

			builder.Property(a => a.Quantity)
				.HasColumnName(nameof(RoomAssetUsage.Quantity))
				.IsRequired();

			builder.Property(a => a.RoomId)
				.HasColumnName(nameof(RoomAssetUsage.RoomId))
				.IsRequired();

			builder.Property(a => a.AssetId)
				.HasColumnName(nameof(RoomAssetUsage.AssetId))
				.IsRequired();

			builder.UseXminAsConcurrencyToken();

			builder
				.HasOne(a => a.Room)
				.WithMany(r => r.AssetUsages)
				.HasForeignKey(a => a.RoomId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.Asset)
				.WithMany(aa => aa.RoomUsages)
				.HasForeignKey(a => a.AssetId)
				.OnDelete(DeleteBehavior.Restrict);

		}
	}
	public class InventoryAssetStatusConfiguration : IEntityTypeConfiguration<InventoryAssetStatus>
	{
		public void Configure(EntityTypeBuilder<InventoryAssetStatus> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(InventoryAssetStatus.Id))
				.IsRequired();

			builder.Property(a => a.Quantity)
				.HasColumnName(nameof(InventoryAssetStatus.Quantity))
				.IsRequired();

			builder.Property(a => a.InventoryId)
				.HasColumnName(nameof(InventoryAssetStatus.InventoryId))
				.IsRequired();

			builder.Property(a => a.AssetId)
				.HasColumnName(nameof(InventoryAssetStatus.AssetId))
				.IsRequired();

			builder
				.HasOne(a => a.Inventory)
				.WithMany(r => r.AssetStatuses)
				.HasForeignKey(a => a.InventoryId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.Asset)
				.WithMany(aa => aa.InventoryStatuses)
				.HasForeignKey(a => a.AssetId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
	public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
	{
		public void Configure(EntityTypeBuilder<Inventory> builder)
		{
			builder.ConfigureChangeTrackingBaseEntity();

			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(Inventory.Id))
				.IsRequired();

			builder.Property(a => a.Date)
				.HasColumnName(nameof(Inventory.Date))
				.IsRequired();

			builder.Property(a => a.WarehouseId)
				.HasColumnName(nameof(Inventory.WarehouseId))
				.IsRequired();

			builder
				.HasOne(a => a.Warehouse)
				.WithMany(w => w.Inventories)
				.HasForeignKey(a => a.WarehouseId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(w => w.AssetStatuses)
				.WithOne(aa => aa.Inventory)
				.HasForeignKey(aa => aa.InventoryId)
				.OnDelete(DeleteBehavior.Restrict);

		}
	}
	public class WarehouseDocumentConfiguration : IEntityTypeConfiguration<WarehouseDocument>
	{
		public void Configure(EntityTypeBuilder<WarehouseDocument> builder)
		{
			builder.ConfigureChangeTrackingBaseEntity();

			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(WarehouseDocument.Id))
				.IsRequired();

			builder.Property(a => a.WarehouseId)
				.HasColumnName(nameof(WarehouseDocument.WarehouseId))
				.IsRequired();

			builder.Property(a => a.TypeKey)
				.HasColumnName(nameof(WarehouseDocument.TypeKey))
				.IsRequired();

			builder.Property(a => a.AssetId)
				.HasColumnName(nameof(WarehouseDocument.AssetId))
				.IsRequired();

			builder.Property(a => a.AvailableQuantityBeforeChange)
				.HasColumnName(nameof(WarehouseDocument.AvailableQuantityBeforeChange))
				.IsRequired();

			builder.Property(a => a.AvailableQuantityChange)
				.HasColumnName(nameof(WarehouseDocument.AvailableQuantityChange))
				.IsRequired();

			builder.Property(a => a.Note)
				.HasColumnName(nameof(WarehouseDocument.Note));

			builder.Property(a => a.ReservedQuantityBeforeChange)
				.HasColumnName(nameof(WarehouseDocument.ReservedQuantityBeforeChange))
				.IsRequired();

			builder.Property(a => a.ReservedQuantityChange)
				.HasColumnName(nameof(WarehouseDocument.ReservedQuantityChange))
				.IsRequired();

			builder
				.HasOne(a => a.Asset)
				.WithMany(aa => aa.WarehouseDocuments)
				.HasForeignKey(a => a.AssetId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.Warehouse)
				.WithMany(w => w.WarehouseDocuments)
				.HasForeignKey(a => a.WarehouseId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
	public class WarehouseDocumentArchiveConfiguration : IEntityTypeConfiguration<WarehouseDocumentArchive>
	{
		public void Configure(EntityTypeBuilder<WarehouseDocumentArchive> builder)
		{
			builder.ConfigureChangeTrackingBaseEntity();

			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(WarehouseDocumentArchive.Id))
				.IsRequired();

			builder.Property(a => a.WarehouseId)
				.HasColumnName(nameof(WarehouseDocumentArchive.WarehouseId))
				.IsRequired();

			builder.Property(a => a.TypeKey)
				.HasColumnName(nameof(WarehouseDocumentArchive.TypeKey))
				.IsRequired();

			builder.Property(a => a.AssetId)
				.HasColumnName(nameof(WarehouseDocumentArchive.AssetId))
				.IsRequired();

			builder.Property(a => a.AvailableQuantityBeforeChange)
				.HasColumnName(nameof(WarehouseDocumentArchive.AvailableQuantityBeforeChange))
				.IsRequired();

			builder.Property(a => a.AvailableQuantityChange)
				.HasColumnName(nameof(WarehouseDocumentArchive.AvailableQuantityChange))
				.IsRequired();

			builder.Property(a => a.Note)
				.HasColumnName(nameof(WarehouseDocumentArchive.Note));

			builder.Property(a => a.ReservedQuantityBeforeChange)
				.HasColumnName(nameof(WarehouseDocumentArchive.ReservedQuantityBeforeChange))
				.IsRequired();

			builder.Property(a => a.ReservedQuantityChange)
				.HasColumnName(nameof(WarehouseDocumentArchive.ReservedQuantityChange))
				.IsRequired();

			builder
				.HasOne(a => a.Asset)
				.WithMany(aa => aa.WarehouseDocumentArchives)
				.HasForeignKey(a => a.AssetId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.Warehouse)
				.WithMany(w => w.WarehouseDocumentArchives)
				.HasForeignKey(a => a.WarehouseId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}

}
