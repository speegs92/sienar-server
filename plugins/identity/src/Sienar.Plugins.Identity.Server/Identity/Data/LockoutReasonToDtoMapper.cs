namespace Sienar.Identity.Data;

/// <summary>
/// Maps from <see cref="LockoutReason"/> to <see cref="LockoutReasonDto"/>
/// </summary>
public class LockoutReasonToDtoMapper : IMapper<LockoutReason, LockoutReasonDto>
{
	/// <inheritdoc />
	public void Map(LockoutReason source, LockoutReasonDto target)
	{
		target.Id = source.Id;
		target.ConcurrencyStamp = source.ConcurrencyStamp;
		target.Reason = source.Reason;
	}
}
