using Microsoft.EntityFrameworkCore;
using Sienar.Data;

namespace TestProject.Data;

public class AppDbContext : SienarDbContext<AppUser>
{
	/// <inheritdoc />
	public AppDbContext(DbContextOptions options)
		: base(options) {}
}