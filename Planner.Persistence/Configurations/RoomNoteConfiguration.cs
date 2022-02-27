using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class RoomNoteConfiguration : IEntityTypeConfiguration<RoomNote>
    {
        public void Configure(EntityTypeBuilder<RoomNote> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(RoomNote.Id))
                .IsRequired();

            builder.Property(a => a.Application)
                .HasColumnName(nameof(RoomNote.Application));
            
            builder.Property(a => a.CreatedAt)
                .HasColumnName(nameof(RoomNote.CreatedAt))
                .IsRequired();
            
            builder.Property(a => a.CreatedById)
                .HasColumnName(nameof(RoomNote.CreatedById))
                .IsRequired();

            builder
                .HasOne(a => a.CreatedBy)
                .WithMany()
                .HasForeignKey(a => a.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(a => a.Expiration)
                .HasColumnName(nameof(RoomNote.Expiration));
            
            builder.Property(a => a.IsArchived)
                .HasColumnName(nameof(RoomNote.IsArchived))
                .IsRequired();
            
            builder.Property(a => a.Note)
                .HasColumnName(nameof(RoomNote.Note))
                .IsRequired();
            
            builder.Property(a => a.RoomId)
                .HasColumnName(nameof(RoomNote.RoomId))
                .IsRequired();

            builder
                .HasOne(a => a.Room)
                .WithMany(r => r.RoomNotes)
                .HasForeignKey(a => a.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(a => a.TaskId)
                .HasColumnName(nameof(RoomNote.TaskId));

            builder
                .HasOne(a => a.Task)
                .WithMany()
                .HasForeignKey(a => a.TaskId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
