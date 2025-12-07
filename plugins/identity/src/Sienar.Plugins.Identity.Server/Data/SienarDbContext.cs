namespace Sienar.Data;

// ReSharper disable NotNullOrRequiredMemberIsNotInitialized
public class SienarDbContext : DbContext, ISienarDbContext
{
	/// <inheritdoc />
	public SienarDbContext() {}

	/// <inheritdoc />
	public SienarDbContext(DbContextOptions options) : base(options) {}

	/// <inheritdoc />
	public DbSet<SienarUser> Users { get; set; }

	/// <inheritdoc />
	public DbSet<SienarRole> Roles { get; set; }

	/// <inheritdoc />
	public DbSet<LockoutReason> LockoutReasons { get; set; }

	/// <inheritdoc />
	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder
			.ApplyConfiguration(new SienarUserEntityConfigurer())
			.ApplyConfiguration(new LockoutReasonEntityConfigurer());
	}
}