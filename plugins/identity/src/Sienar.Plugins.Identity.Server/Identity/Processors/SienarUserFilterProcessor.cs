#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class SienarUserFilterProcessor : IEfFilterProcessor<SienarUser>
{
	public IQueryable<SienarUser> Search(IQueryable<SienarUser> dataset, Filter filter)
	{
		if (string.IsNullOrEmpty(filter.SearchTerm))
		{
			return dataset;
		}

		var searchTerm = filter.SearchTerm.ToNormalized();

		return dataset.Where(
			s => s.NormalizedUsername.Contains(searchTerm)
			|| s.NormalizedEmail.Contains(searchTerm)
			|| !string.IsNullOrEmpty(s.NormalizedPendingEmail) && s.NormalizedPendingEmail.Contains(searchTerm));
	}

	public IQueryable<SienarUser> ProcessIncludes(IQueryable<SienarUser> dataset, Filter filter)
	{
		if (filter.Includes.Count == 0)
		{
			return dataset;
		}

		if (filter.Includes.Contains(nameof(SienarUser.LockoutReasons)))
		{
			dataset = dataset.Include(u => u.LockoutReasons);
		}

		return dataset;
	}

	public Expression<Func<SienarUser, object>> GetSortPredicate(string? sortName) => sortName switch
	{
		nameof(SienarUser.Username) => u => u.Username,
		nameof(SienarUser.Email) => u => u.Email,
		nameof(SienarUser.PendingEmail) => u => u.PendingEmail!,
		_ => u => u.Username
	};
}