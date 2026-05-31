namespace Sienar.Infrastructure;

/// <summary>
/// Provides the key of any number of menus to render for a given page
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class MenusAttribute : Attribute
{
	/// <summary>
	/// The names of the menus to render
	/// </summary>
	public List<string> Names { get; set; }

	/// <summary>
	/// Creates a new instance of <c>MenusAttribute</c>
	/// </summary>
	/// <param name="names">The names of menus to render for the given page</param>
	public MenusAttribute(params string[] names)
	{
		Names = names.ToList();
	}
}
