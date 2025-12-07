namespace Sienar.Identity.Results;

public class AccountLockoutResult : IResult
{
	public List<LockoutReason> LockoutReasons { get; set; }
	public DateTime? LockoutEnd { get; set; }
}