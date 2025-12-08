using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TestProject.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
	/// <inheritdoc />
	public AppDbContext CreateDbContext(string[] args)
	{
		var builder = new DbContextOptionsBuilder<AppDbContext>();

		builder.UseSienarDb();

		return new AppDbContext(builder.Options);
	}
}