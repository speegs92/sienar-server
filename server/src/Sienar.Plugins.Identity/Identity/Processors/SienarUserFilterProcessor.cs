#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class SienarUserFilterProcessor<T> : IEfFilterProcessor<T>
	where T : class, ISienarIdentityUser<T>
{
	public IQueryable<T> Search(IQueryable<T> dataset, Filter filter)
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

	public IQueryable<T> ProcessIncludes(IQueryable<T> dataset, Filter filter)
	{
		if (filter.Includes.Count == 0)
		{
			return dataset;
		}

		if (filter.Includes.Contains(nameof(ISienarIdentityUser<T>.LockoutReasons)))
		{
			dataset = dataset.Include(u => u.LockoutReasons);
		}

		return dataset;
	}

	public Expression<Func<T, object>> GetSortPredicate(string? sortName) => sortName switch
	{
		nameof(ISienarIdentityUser<T>.Username) => u => u.Username,
		nameof(ISienarIdentityUser<T>.Email) => u => u.Email,
		nameof(ISienarIdentityUser<T>.PendingEmail) => u => u.PendingEmail!,
		_ => u => u.Username
	};
}