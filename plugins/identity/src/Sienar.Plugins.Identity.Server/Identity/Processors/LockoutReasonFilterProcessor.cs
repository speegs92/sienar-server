#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class LockoutReasonFilterProcessor<T> : IEfFilterProcessor<LockoutReason<T>>
	where T : class, ISienarIdentityUser<T>
{
	public IQueryable<LockoutReason<T>> Search(IQueryable<LockoutReason<T>> dataset, Filter filter)
		=> string.IsNullOrEmpty(filter.SearchTerm)
			? dataset
			: dataset.Where(r => r.NormalizedReason.Contains(filter.SearchTerm.ToNormalized()));

	public Expression<Func<LockoutReason<T>, object>> GetSortPredicate(string? sortName) => r => r.Reason;
}