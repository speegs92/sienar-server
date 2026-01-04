namespace Sienar.Identity;

/// <summary>
/// Reads a user from the database
/// </summary>
[EntityName(Singular = "user", Plural = "users")]
public class ViewUserDto : IEntity
{
	/// <inheritdoc />
	public int Id { get; set; }

	/// <inheritdoc />
	public Guid ConcurrencyStamp { get; set; }

	/// <summary>
	/// The user's password
	/// </summary>
	public string Username { get; set; } = string.Empty;

	/// <summary>
	/// The user's email address
	/// </summary>
	public string Email { get; set; } = string.Empty;

	/// <summary>
	/// Whether the user's email address is confirmed
	/// </summary>
	public bool EmailConfirmed { get; set; }

	/// <summary>
	/// The end date of the user's lockout period. <c>null</c> if the user is not locked out
	/// </summary>
	public DateTime? LockoutEnd { get; set; }

	/// <summary>
	/// The user's roles
	/// </summary>
	public List<string> Roles { get; set; } = [];
}
