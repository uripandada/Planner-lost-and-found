using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class AssetFileConfiguration : IEntityTypeConfiguration<AssetFile>
    {
        public void Configure(EntityTypeBuilder<AssetFile> builder)
        {
            builder.HasKey(a => new { a.AssetId, a.FileId });

            builder.Property(a => a.IsPrimaryImage)
                .HasColumnName(nameof(AssetFile.IsPrimaryImage))
                .IsRequired();

            builder.Property(a => a.IsQrCodeImage)
                .HasColumnName(nameof(AssetFile.IsQrCodeImage))
                .IsRequired();

            builder
                .HasOne(af => af.Asset)
                .WithMany(a => a.AssetFiles)
                .HasForeignKey(af => af.AssetId);

            builder
                .HasOne(af => af.File)
                .WithMany()
                .HasForeignKey(af => af.FileId);
        }
    }
}
