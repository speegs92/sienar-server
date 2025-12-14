namespace Sienar.Identity.Results;

public class AccountLockoutResult : IResult
{
	public List<LockoutReasonDto> LockoutReasons { get; set; }
	public DateTime? LockoutEnd { get; set; }
}