using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class AssetModelConfiguration : IEntityTypeConfiguration<AssetModel>
    {
        public void Configure(EntityTypeBuilder<AssetModel> builder)
        {
            builder.ConfigureChangeTrackingBaseEntity();

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(AssetModel.Id))
                .IsRequired();

            builder.Property(a => a.Name)
                .HasColumnName(nameof(AssetModel.Name))
                .IsRequired();

            //builder.Property(a => a.IsAvailableToHousekeeping)
            //    .HasColumnName(nameof(AssetModel.IsAvailableToHousekeeping))
            //    .IsRequired();

            //builder.Property(a => a.IsAvailableToMaintenance)
            //    .HasColumnName(nameof(AssetModel.IsAvailableToMaintenance))
            //    .IsRequired();

            //builder
            //    .HasOne(am => am.Asset)
            //    .WithMany(a => a.Models)
            //    .HasForeignKey(am => am.AssetId)
            //    .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(am => am.RoomAssetModels)
                .WithOne(ram => ram.AssetModel)
                .HasForeignKey(am => am.AssetModelId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder
            //    .HasMany(a => a.Actions)
            //    .WithOne(aa => aa.AssetModel)
            //    .HasForeignKey(aa => aa.AssetModelId)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
