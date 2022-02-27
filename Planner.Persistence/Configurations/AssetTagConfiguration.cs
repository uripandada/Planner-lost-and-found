using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class AssetTagConfiguration : IEntityTypeConfiguration<AssetTag>
    {
        public void Configure(EntityTypeBuilder<AssetTag> builder)
        {
            builder.HasKey(a => new { a.AssetId, a.TagKey });

            builder
                .HasOne(at => at.Asset)
                .WithMany(a => a.AssetTags)
                .HasForeignKey(at => at.AssetId);

            builder
                .HasOne(at => at.Tag)
                .WithMany()
                .HasForeignKey(at => at.TagKey);
        }
    }
}
