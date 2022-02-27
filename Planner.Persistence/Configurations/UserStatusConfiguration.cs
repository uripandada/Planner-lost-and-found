using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Common.Enums;
using Planner.Domain.Entities;
using System;

namespace Planner.Persistence.Configurations
{
	//public class UserStatusConfiguration : IEntityTypeConfiguration<UserStatus>
 //   {
 //       public void Configure(EntityTypeBuilder<UserStatus> builder)
 //       {
 //           builder.HasKey(a => a.Id);

 //           builder.Property(a => a.Id)
 //               .HasColumnName(nameof(UserStatus.Id))
 //               .IsRequired();

 //           builder.Property(a => a.CreatedAt)
 //               .HasColumnName(nameof(UserStatus.CreatedAt))
 //               .IsRequired();

 //           builder.Property(a => a.RoomId)
 //               .HasColumnName(nameof(UserStatus.RoomId));

 //           builder.Property(a => a.StatusDescription)
 //               .HasColumnName(nameof(UserStatus.StatusDescription))
 //               .IsRequired();

 //           builder.Property(a => a.Status)
 //            .HasColumnName(nameof(UserStatus.Status))
 //            .IsRequired()
 //            .HasConversion(enumValue => enumValue.ToString(),
 //               stringValue => (UserStatusType)Enum.Parse(typeof(UserStatusType), stringValue));

 //           builder
 //               .HasOne(a => a.User)
 //               .WithOne(u => u.UserStatus)
 //               .HasForeignKey<UserStatus>(a => a.Id)
 //               .OnDelete(DeleteBehavior.Restrict);

 //           builder
 //               .HasOne(a => a.Room)
 //               .WithMany()
 //               .HasForeignKey(a => a.RoomId)
 //               .OnDelete(DeleteBehavior.Restrict);
 //       }
 //   }	
}
