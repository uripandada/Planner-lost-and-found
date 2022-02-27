using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Persistence.Configurations
{
    public class OnGuardConfiguration : IEntityTypeConfiguration<OnGuard>
    {
        public void Configure(EntityTypeBuilder<OnGuard> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Files).WithOne(x => x.OnGuard).HasForeignKey(x => x.OnGuardId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
