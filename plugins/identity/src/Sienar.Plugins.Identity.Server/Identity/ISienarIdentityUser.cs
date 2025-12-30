namespace Sienar.Identity;

/// <summary>
/// The <c>Sienar.Plugins.Identity</c> user
/// </summary>
public interface ISienarIdentityUser<T> : IUser
	where T : class, ISienarIdentityUser<T>
{
	/// <summary>
	/// The user's salted and hashed password
	/// </summary>
	string PasswordHash { get; set; }

	/// <summary>
	/// The number of failed login attempts
	/// </summary>
	int LoginFailedCount { get; set; }

	/// <summary>
	/// The end date of the lockout period
	/// </summary>
	DateTime? LockoutEnd { get; set; }

	/// <summary>
	/// A list of verification codes for the user
	/// </summary>
	List<VerificationCode> VerificationCodes { get; set; }

	/// <summary>
	/// A list of reasons why the user is locked out. Empty if the user is not locked out
	/// </summary>
	List<LockoutReason<T>> LockoutReasons { get; set; }

	/// <summary>
	/// Whether the email address for the user has been confirmed
	/// </summary>
	bool EmailConfirmed { get; set; }

	/// <summary>
	/// The user's pending email address
	/// </summary>
	string? PendingEmail { get; set; }

	/// <summary>
	/// The user's normalized pending email address
	/// </summary>
	string? NormalizedPendingEmail { get; set; }
}