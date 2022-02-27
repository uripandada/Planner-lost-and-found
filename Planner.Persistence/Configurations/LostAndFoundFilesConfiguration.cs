using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Persistence.Configurations
{
    public class LostAndFoundFilesConfiguration : IEntityTypeConfiguration<LostAndFoundFile>
    {
        public void Configure(EntityTypeBuilder<LostAndFoundFile> builder)
        {
            builder.HasKey(x => new { x.LostAndFoundId, x.FileId });
            builder.HasOne(x => x.File).WithMany().HasForeignKey(x => x.FileId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.LostAndFound).WithMany(x => x.Files).HasForeignKey(x => x.LostAndFoundId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
