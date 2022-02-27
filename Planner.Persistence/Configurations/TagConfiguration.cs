using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ConfigureChangeTrackingBaseEntity();

            builder.HasKey(a => a.Key);

            builder.Property(a => a.Key)
                .HasColumnName(nameof(Tag.Key))
                .IsRequired();

            builder.Property(a => a.Value)
                .HasColumnName(nameof(Tag.Value))
                .IsRequired();
        }
    }
}
