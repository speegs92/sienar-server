namespace Sienar.Identity;

[EntityName(Singular = "user", Plural = "users")]
public class SienarUser : EntityBase
{
#region Security

	/// <summary>
	/// The user's username
	/// </summary>
	[PersonalData]
	public string Username { get; set; } = default!;

	/// <summary>
	/// The user's normalized username
	/// </summary>
	public string NormalizedUsername { get; set; } = string.Empty;

	/// <summary>
	/// The user's salted and hashed password
	/// </summary>
	public string PasswordHash { get; set; } = string.Empty;

	/// <summary>
	/// The number of failed login attempts
	/// </summary>
	public int LoginFailedCount { get; set; }

	/// <summary>
	/// The end date of the lockout period
	/// </summary>
	public DateTime? LockoutEnd { get; set; }

	/// <summary>
	/// A list of verification codes
	/// </summary>
	public List<VerificationCode> VerificationCodes { get; set; } = [];

	public List<SienarRole> Roles { get; set; } = [];

	public List<LockoutReason> LockoutReasons { get; set; } = [];

#endregion

#region Contact information

	/// <summary>
	/// The user's email address
	/// </summary>
	[PersonalData]
	[EmailAddress]
	public string Email { get; set; } = string.Empty;

	/// <summary>
	/// The user's normalized email address
	/// </summary>
	public string NormalizedEmail { get; set; } = string.Empty;

	/// <summary>
	/// Whether the email address for the user has been confirmed
	/// </summary>
	public bool EmailConfirmed { get; set; }

	/// <summary>
	/// The user's pending email address
	/// </summary>
	[PersonalData]
	public string? PendingEmail { get; set; }

	/// <summary>
	/// The user's normalized pending email address
	/// </summary>
	public string? NormalizedPendingEmail { get; set; }

#endregion

	/// <inheritdoc/>
	public override string ToString() => Username;
}
