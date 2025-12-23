using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sienar.Identity.Data;

internal class LockoutReasonEntityConfigurer<T> : IEntityTypeConfiguration<LockoutReason<T>>
	where T : class, ISienarIdentityUser<T>
{
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<LockoutReason<T>> builder)
	{
		builder
			.HasIndex(l => l.NormalizedReason)
			.IsUnique();
		builder
			.Property(l => l.NormalizedReason)
			.HasMaxLength(255)
			.IsRequired();
		builder
			.Property(l => l.Reason)
			.HasMaxLength(255)
			.IsRequired();
	}
}