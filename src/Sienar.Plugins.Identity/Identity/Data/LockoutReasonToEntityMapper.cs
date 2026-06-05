namespace Sienar.Identity.Data;

/// <summary>
/// Maps from <see cref="LockoutReasonDto"/> to <see cref="LockoutReason"/>
/// </summary>
public class LockoutReasonToEntityMapper<T> : IMapper<LockoutReasonDto, LockoutReason<T>>
	where T : class, ISienarIdentityUser<T>
{
	/// <inheritdoc />
	public void Map(LockoutReasonDto source, LockoutReason<T> target)
	{
		target.Reason = source.Reason;
		target.NormalizedReason = source.Reason.ToNormalized();
	}

	/// <inheritdoc />
	public void MapToDto(LockoutReasonDto dto, LockoutReason<T> entity)
	{
		dto.Id = entity.Id;
		dto.ConcurrencyStamp = entity.ConcurrencyStamp;
		dto.Reason = entity.Reason;
	}
}
