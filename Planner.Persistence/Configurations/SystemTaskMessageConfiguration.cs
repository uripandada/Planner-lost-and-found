using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class SystemTaskMessageConfiguration : IEntityTypeConfiguration<SystemTaskMessage>
    {
        public void Configure(EntityTypeBuilder<SystemTaskMessage> builder)
        {
            builder.ConfigureChangeTrackingBaseEntity();

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(SystemTaskMessage.Id))
                .IsRequired();

            builder
                .Property(a => a.Message)
                .HasColumnName(nameof(SystemTaskMessage.Message))
                .IsRequired();

            builder
                .Property(a => a.SystemTaskId)
                .HasColumnName(nameof(SystemTaskMessage.SystemTaskId))
                .IsRequired();

            builder
                .HasOne(a => a.SystemTask)
                .WithMany(t => t.Messages)
                .HasForeignKey(a => a.SystemTaskId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
