using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Persistence.Configurations
{
    public class OnGuardFilesConfiguration : IEntityTypeConfiguration<OnGuardFile>
    {
        public void Configure(EntityTypeBuilder<OnGuardFile> builder)
        {
            builder.HasKey(x => new { x.OnGuardId, x.FileId });
            builder.HasOne(x => x.OnGuard).WithMany(x => x.Files).HasForeignKey(x => x.OnGuardId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.File).WithMany().HasForeignKey(x => x.FileId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
