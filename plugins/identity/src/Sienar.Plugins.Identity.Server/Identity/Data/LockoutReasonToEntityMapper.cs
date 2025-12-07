namespace Sienar.Identity.Data;

/// <summary>
/// Maps from <see cref="LockoutReasonDto"/> to <see cref="LockoutReason"/>
/// </summary>
public class LockoutReasonToEntityMapper : IMapper<LockoutReasonDto, LockoutReason>
{
	/// <inheritdoc />
	public void Map(LockoutReasonDto source, LockoutReason target)
	{
		target.Reason = source.Reason;
		target.NormalizedReason = source.Reason.ToNormalized();
	}

	/// <inheritdoc />
	public void MapToDto(LockoutReasonDto dto, LockoutReason entity)
	{
		dto.Id = entity.Id;
		dto.ConcurrencyStamp = entity.ConcurrencyStamp;
		dto.Reason = entity.Reason;
	}
}
