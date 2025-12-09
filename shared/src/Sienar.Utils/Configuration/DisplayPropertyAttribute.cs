namespace Sienar.Configuration;

/// <summary>
/// Designates the property whose getter should be called to access the name of an entity or DTO instance
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DisplayPropertyAttribute : Attribute
{
	/// <summary>
	/// The property name used to retrieve the display name of an entity or DTO
	/// </summary>
	public string Property { get; }

	/// <summary>
	/// Designates the property whose getter should be called to access the name of an entity or DTO instance
	/// </summary>
	/// <param name="property">The property name used to retrieve the display name of an entity or DTO</param>
	public DisplayPropertyAttribute(string property)
		=> Property = property;
}