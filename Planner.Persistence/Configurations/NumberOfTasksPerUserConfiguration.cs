using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class NumberOfTasksPerUserConfiguration : IEntityTypeConfiguration<NumberOfTasksPerUser>
    {
        public void Configure(EntityTypeBuilder<NumberOfTasksPerUser> builder)
        {
            builder
                .HasNoKey()
                .ToTable(null)
                .ToView("ViewNumberOfTasksPerUserConfiguration");
        }
    }
}
