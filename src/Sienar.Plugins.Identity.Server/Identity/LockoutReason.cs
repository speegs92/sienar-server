namespace Sienar.Identity;

/// <summary>
/// A reason why a user might be locked out
/// </summary>
[EntityName(Singular = "lockout reason", Plural = "lockout reasons")]
public class LockoutReason<T> : IEntity
	where T : class, ISienarIdentityUser<T>
{
	/// <inheritdoc />
	public int Id { get; set; }

	/// <inheritdoc />
	public Guid ConcurrencyStamp { get; set; }

	/// <summary>
	/// The reason why a user might be locked out
	/// </summary>
	public string Reason { get; set; } = string.Empty;

	/// <summary>
	/// The normalized reason why a user might be locked out
	/// </summary>
	public string NormalizedReason { get; set; } = string.Empty;

	/// <summary>
	/// The users who are locked out for this reason
	/// </summary>
	public List<T> Users { get; set; } = [];

	/// <inheritdoc />
	public override string ToString() => Reason;
}