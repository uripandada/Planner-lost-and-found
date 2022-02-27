using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class FileConfiguration : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> builder)
        {
            builder.ConfigureChangeTrackingBaseEntity();

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(File.Id))
                .IsRequired();

            builder.Property(a => a.FileTypeKey)
                .HasColumnName(nameof(File.FileTypeKey))
                .HasDefaultValue("UNKNOWN")
                .IsRequired();

            builder.Property(a => a.FileName)
                .HasColumnName(nameof(File.FileName))
                .IsRequired();

            builder.Property(a => a.FileData)
                .HasColumnName(nameof(File.FileData))
                .IsRequired();
        }
    }
}
