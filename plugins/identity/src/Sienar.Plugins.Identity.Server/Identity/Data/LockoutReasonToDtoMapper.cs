namespace Sienar.Identity.Data;

/// <summary>
/// Maps from <see cref="LockoutReason{T}"/> to <see cref="LockoutReasonDto"/>
/// </summary>
public class LockoutReasonToDtoMapper<T> : IMapper<LockoutReason<T>, LockoutReasonDto>
	where T : class, ISienarIdentityUser<T>
{
	/// <inheritdoc />
	public void Map(LockoutReason<T> source, LockoutReasonDto target)
	{
		target.Id = source.Id;
		target.ConcurrencyStamp = source.ConcurrencyStamp;
		target.Reason = source.Reason;
	}
}
