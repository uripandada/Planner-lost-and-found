using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Common.Enums;
using Planner.Domain.Entities;
using System;

namespace Planner.Persistence.Configurations
{
	//public class UserStatusHistoryConfiguration : IEntityTypeConfiguration<UserStatusHistory>
 //   {
 //       public void Configure(EntityTypeBuilder<UserStatusHistory> builder)
 //       {
 //           builder.HasKey(a => a.Id);

 //           builder.Property(a => a.Id)
 //               .HasColumnName(nameof(UserStatusHistory.Id))
 //               .IsRequired();

 //           builder.Property(a => a.CreatedAt)
 //               .HasColumnName(nameof(UserStatusHistory.CreatedAt))
 //               .IsRequired();

 //           builder.Property(a => a.RoomId)
 //               .HasColumnName(nameof(UserStatusHistory.RoomId));

 //           builder.Property(a => a.StatusDescription)
 //               .HasColumnName(nameof(UserStatusHistory.StatusDescription))
 //               .IsRequired();

 //           builder.Property(a => a.Status)
 //            .HasColumnName(nameof(UserStatusHistory.Status))
 //            .IsRequired()
 //            .HasConversion(enumValue => enumValue.ToString(),
 //               stringValue => (UserStatusType)Enum.Parse(typeof(UserStatusType), stringValue));

 //           builder.Property(a => a.UserId)
 //               .HasColumnName(nameof(UserStatusHistory.UserId))
 //               .IsRequired();

 //           builder
 //               .HasOne(a => a.User)
 //               .WithMany()
 //               .HasForeignKey(a => a.UserId)
 //               .OnDelete(DeleteBehavior.Restrict);

 //           builder
 //               .HasOne(a => a.Room)
 //               .WithMany()
 //               .HasForeignKey(a => a.RoomId)
 //               .OnDelete(DeleteBehavior.Restrict);
 //       }
 //   }
}
