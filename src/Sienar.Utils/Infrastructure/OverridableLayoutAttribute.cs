namespace Sienar.Infrastructure;

/// <summary>
/// Provides the enum key of a page's overridable layout to Sienar
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class OverridableLayoutAttribute : Attribute
{
	/// <summary>
	/// The key of the overridable layout
	/// </summary>
	public string LayoutKey { get; init; }

	/// <summary>
	/// Creates a new instance of <c>OverridableLayoutAttribute</c>
	/// </summary>
	/// <param name="layoutKey">The key of the overridable layout</param>
	public OverridableLayoutAttribute(string layoutKey)
	{
		LayoutKey = layoutKey;
	}
}
