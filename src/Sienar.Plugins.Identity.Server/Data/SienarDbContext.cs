namespace Sienar.Data;

// ReSharper disable NotNullOrRequiredMemberIsNotInitialized
public class SienarDbContext<TUser> : DbContext, ISienarDbContext<TUser>
	where TUser : class, ISienarIdentityUser<TUser>
{
	/// <inheritdoc />
	public SienarDbContext() {}

	/// <inheritdoc />
	public SienarDbContext(DbContextOptions options) : base(options) {}

	/// <inheritdoc />
	public DbSet<TUser> Users { get; set; }

	/// <inheritdoc />
	public DbSet<LockoutReason<TUser>> LockoutReasons { get; set; }

	/// <inheritdoc />
	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder
			.ApplyConfiguration(new SienarUserEntityConfigurer<TUser>())
			.ApplyConfiguration(new LockoutReasonEntityConfigurer<TUser>());
	}
}