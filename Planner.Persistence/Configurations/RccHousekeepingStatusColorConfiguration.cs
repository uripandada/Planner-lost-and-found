using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Common.Enums;
using Planner.Domain.Entities;
using System;

namespace Planner.Persistence.Configurations
{
	public class RccHousekeepingStatusColorConfiguration : IEntityTypeConfiguration<RccHousekeepingStatusColor>
	{
		public void Configure(EntityTypeBuilder<RccHousekeepingStatusColor> builder)
		{
			builder.HasKey(h => h.RccCode);

			builder.Property(h => h.RccCode)
				.HasColumnName(nameof(RccHousekeepingStatusColor.RccCode))
				.HasConversion(
					a => a.ToString(),
					a => (RccHousekeepingStatusCode)Enum.Parse(typeof(RccHousekeepingStatusCode), a)
				)
				.IsRequired();

			builder.Property(h => h.ColorHex)
				.HasColumnName(nameof(RccHousekeepingStatusColor.ColorHex))
				.IsRequired();
		}
	}
}
