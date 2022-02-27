using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class RoomAssetModelConfiguration : IEntityTypeConfiguration<RoomAssetModel>
    {
        public void Configure(EntityTypeBuilder<RoomAssetModel> builder)
        {
            builder.HasKey(a => new { a.RoomId, a.AssetModelId });

            builder.Property(a => a.RoomId)
                .HasColumnName(nameof(RoomAssetModel.RoomId))
                .IsRequired();

            builder.Property(a => a.AssetModelId)
                .HasColumnName(nameof(RoomAssetModel.AssetModelId))
                .IsRequired();

            builder.Property(a => a.Quantity)
                .HasColumnName(nameof(RoomAssetModel.Quantity))
                .IsRequired();

            builder
                .HasOne(ram => ram.AssetModel)
                .WithMany(am => am.RoomAssetModels)
                .HasForeignKey(ram => ram.AssetModelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(ram => ram.Room)
                .WithMany(r => r.RoomAssetModels)
                .HasForeignKey(ram => ram.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
