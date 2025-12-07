#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class LockoutReasonFilterProcessor : IEfFilterProcessor<LockoutReason>
{
	public IQueryable<LockoutReason> Search(IQueryable<LockoutReason> dataset, Filter filter)
		=> string.IsNullOrEmpty(filter.SearchTerm)
			? dataset
			: dataset.Where(r => r.NormalizedReason.Contains(filter.SearchTerm.ToNormalized()));

	public Expression<Func<LockoutReason, object>> GetSortPredicate(string? sortName) => r => r.Reason;
}