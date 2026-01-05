namespace Sienar.Data;

/// <summary>
/// A basic Sienar entity
/// </summary>
public interface IEntity
{
	/// <summary>
	/// The entity's primary key
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// The entity's concurrency stamp
	/// </summary>
	public Guid ConcurrencyStamp { get; set; }
}