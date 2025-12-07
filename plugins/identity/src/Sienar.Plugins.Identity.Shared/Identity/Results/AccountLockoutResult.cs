using System;
using System.Collections.Generic;
using Sienar.Data;

namespace Sienar.Identity.Results;

public class AccountLockoutResult : IResult
{
	public List<LockoutReason> LockoutReasons { get; set; }
	public DateTime? LockoutEnd { get; set; }
}