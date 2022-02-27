using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Persistence.Configurations
{
    public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
    {
        public void Configure(EntityTypeBuilder<UserGroup> builder)
        {
            builder.ConfigureChangeTrackingBaseEntity();

            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.UserSubGroups).WithOne(x => x.UserGroup).HasForeignKey(x => x.UserGroupId).OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(group => group.Users)
                .WithOne(user => user.UserGroup)
                .HasForeignKey(user => user.UserGroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
