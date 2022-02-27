using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;
using Planner.Common.Enums;
using System;

namespace Planner.Persistence.Configurations
{
	//public class CleaningPlanItemEventConfiguration : IEntityTypeConfiguration<CleaningPlanItemEvent>
 //   {
 //       public void Configure(EntityTypeBuilder<CleaningPlanItemEvent> builder)
 //       {
 //           builder.HasKey(a => a.Id);

 //           builder.Property(a => a.Id)
 //               .HasColumnName(nameof(CleaningPlanItemEvent.Id))
 //               .IsRequired();

 //           builder.Property(a => a.EventType)
 //               .HasColumnName(nameof(CleaningPlanItemEvent.EventType))
 //               .HasConversion(
 //                   a => a.ToString(),
 //                   a => (CleaningEventType)Enum.Parse(typeof(CleaningEventType), a)
 //               );

 //           builder.Property(a => a.IsSystemEvent)
 //               .HasColumnName(nameof(CleaningPlanItemEvent.IsSystemEvent))
 //               .IsRequired();

 //           builder.Property(a => a.CreatedById)
 //               .HasColumnName(nameof(CleaningPlanItemEvent.CreatedById));

 //           builder.Property(a => a.CreatedAt)
 //               .HasColumnName(nameof(CleaningPlanItemEvent.CreatedAt))
 //               .IsRequired();

 //           builder.Property(a => a.Message)
 //               .HasColumnName(nameof(CleaningPlanItemEvent.Message));
            
 //           builder.Property(a => a.OldState)
 //               .HasColumnName(nameof(CleaningPlanItemEvent.OldState));
            
 //           builder.Property(a => a.NewState)
 //               .HasColumnName(nameof(CleaningPlanItemEvent.NewState));

 //           builder
 //               .HasOne(a => a.CreatedBy)
 //               .WithMany()
 //               .HasForeignKey(a => a.CreatedById)
 //               .OnDelete(DeleteBehavior.Restrict);

 //           builder
 //               .HasOne(h => h.CleaningPlanItem)
 //               .WithMany(i => i.History)
 //               .HasForeignKey(h => h.CleaningPlanItemId)
 //               .OnDelete(DeleteBehavior.Restrict);
 //       }
 //   }
}
