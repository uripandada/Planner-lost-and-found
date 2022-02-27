using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Planner.Persistence.Configurations
{
    public class UserSubGroupConfiguration : IEntityTypeConfiguration<UserSubGroup>
    {
        public void Configure(EntityTypeBuilder<UserSubGroup> builder)
        {
            builder.ConfigureChangeTrackingBaseEntity();

            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.UserGroup).WithMany(x=>x.UserSubGroups).HasForeignKey(x => x.UserGroupId).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
            builder.HasMany(x => x.Users).WithOne(x => x.UserSubGroup).HasForeignKey(x => x.UserSubGroupId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
