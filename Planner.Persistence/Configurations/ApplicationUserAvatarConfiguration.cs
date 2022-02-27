using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class ApplicationUserAvatarConfiguration : IEntityTypeConfiguration<ApplicationUserAvatar>
	{
		public void Configure(EntityTypeBuilder<ApplicationUserAvatar> builder)
		{
			builder.HasKey(ai => ai.Id);

			builder.Property(ai => ai.Id)
				.HasColumnName(nameof(ApplicationUserAvatar.Id))
				.IsRequired();

			builder.Property(ai => ai.File)
				.HasColumnName(nameof(ApplicationUserAvatar.File))
				.IsRequired();

			builder.Property(ai => ai.FileName)
				.HasColumnName(nameof(ApplicationUserAvatar.FileName))
				.IsRequired();

			builder.Property(ai => ai.FileUrl)
				.HasColumnName(nameof(ApplicationUserAvatar.FileUrl));

			builder
				.HasOne(ai => ai.User)
				.WithOne(u => u.Avatar)
				.HasForeignKey<ApplicationUserAvatar>(ai => ai.Id)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
