namespace Sienar.Identity;

/// <summary>
/// Creates or updates user in the database
/// </summary>
[EntityName(Singular = "user", Plural = "users")]
[RestEndpoint("users")]
public class UpsertUserDto : EntityBase
{
	/// <summary>
	/// The user's username
	/// </summary>
	[Required]
	[DisplayName("Username")]
	[StringLength(50, MinimumLength = 6, ErrorMessage = "Username must be between 6 and 50 characters long")]
	public string Username { get; set; } = string.Empty;

	/// <summary>
	/// The user's email address
	/// </summary>
	[Required]
	[DisplayName("Email")]
	[EmailAddress]
	[StringLength(100, MinimumLength = 6, ErrorMessage = "Email must be between 6 and 100 characters long")]
	public string Email { get; set; } = string.Empty;

	/// <summary>
	/// The user's password
	/// </summary>
	[DisplayName("Password")]
	[DataType(DataType.Password)]
	public string? Password { get; set; } = string.Empty;

	/// <summary>
	/// The user's password, repeated for confirmation
	/// </summary>
	[DisplayName("Confirm password")]
	[DataType(DataType.Password)]
	[Compare(nameof(Password), ErrorMessage = "The passwords do not match")]
	public string? ConfirmPassword { get; set; } = string.Empty;

	/// <summary>
	/// The user's roles
	/// </summary>
	public List<string> Roles { get; set; } = [];
}
