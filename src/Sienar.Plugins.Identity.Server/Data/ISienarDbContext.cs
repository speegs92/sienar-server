namespace Sienar.Data;

public interface ISienarDbContext<TUser> : IDbContext
	where TUser : class, ISienarIdentityUser<TUser>
{
	DbSet<TUser> Users { get; set; }
	DbSet<LockoutReason<TUser>> LockoutReasons { get; set; }
}