using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public static class EntityConfigurationExtensions
	{
		public static void ConfigureBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : BaseEntity
		{
			builder.Property(e => e.HotelId)
				.HasColumnName(nameof(BaseEntity.HotelId))
				.IsRequired();

			builder
				.HasOne(x => x.Hotel)
				.WithMany()
				.HasForeignKey(x => x.HotelId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.ConfigureChangeTrackingBaseEntity();
		}

		public static void ConfigureChangeTrackingBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : ChangeTrackingBaseEntity
		{
			builder.Property(e => e.CreatedById)
				.HasColumnName(nameof(ChangeTrackingBaseEntity.CreatedById));

			builder.Property(e => e.CreatedAt)
				.HasColumnName(nameof(ChangeTrackingBaseEntity.CreatedAt))
				.IsRequired()
				.HasDefaultValueSql("now()");

			builder.Property(e => e.ModifiedById)
				.HasColumnName(nameof(ChangeTrackingBaseEntity.ModifiedById));

			builder.Property(e => e.ModifiedAt)
				.HasColumnName(nameof(ChangeTrackingBaseEntity.ModifiedAt))
				.IsRequired()
				.HasDefaultValueSql("now()");

			builder
				.HasOne(x => x.CreatedBy)
				.WithMany()
				.HasForeignKey(x => x.CreatedById)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(x => x.ModifiedBy)
				.WithMany()
				.HasForeignKey(x => x.ModifiedById)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
