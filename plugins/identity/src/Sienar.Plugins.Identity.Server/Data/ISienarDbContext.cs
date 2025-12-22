namespace Sienar.Data;

public interface ISienarDbContext : IDbContext
{
	DbSet<SienarUser> Users { get; set; }
	DbSet<LockoutReason> LockoutReasons { get; set; }
}