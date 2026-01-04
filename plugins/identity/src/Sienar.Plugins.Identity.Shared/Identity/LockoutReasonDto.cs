namespace Sienar.Identity;

/// <summary>
/// A reason why a user might be locked out
/// </summary>
[EntityName(Singular = "lockout reason", Plural = "lockout reasons")]
public class LockoutReasonDto : IEntity
{
	/// <inheritdoc />
	public int Id { get; set; }

	/// <inheritdoc />
	public Guid ConcurrencyStamp { get; set; }

	/// <summary>
	/// The reason why a user might be locked out
	/// </summary>
	public string Reason { get; set; } = string.Empty;
}
