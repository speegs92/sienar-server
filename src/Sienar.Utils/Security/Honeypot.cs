namespace Sienar.Security;

/// <summary>
/// Represents a DTO that has honeypot capabilities
/// </summary>
[ExcludeFromCodeCoverage]
public class Honeypot : IEntity
{
	/// <inheritdoc />
	public int Id { get; set; }

	/// <inheritdoc />
	public Guid ConcurrencyStamp { get; set; }

	/// <summary>
	/// Used to detect spambot submissions
	/// </summary>
	public string? SecretKeyField { get; set; }

	/// <summary>
	/// Used to determine how long a form took an agent to complete
	/// </summary>
	public TimeSpan TimeToComplete { get; set; }
}