using System.Text;

namespace Sienar.Html;

/// <summary>
/// Generates CSS strings for use in Blazor components
/// </summary>
public readonly struct CssBuilder
{
	private readonly StringBuilder _sb = new();

	/// <summary>
	/// Creates a new instance of <c>CssBuilder</c>
	/// </summary>
	public CssBuilder() {}

	/// <summary>
	/// Adds the given classes to the <c>CssBuilder</c>
	/// </summary>
	/// <param name="classes">The classes to add</param>
	/// <returns>The <c>CssBuilder</c></returns>
	public CssBuilder AddClasses(string classes)
	{
		_sb.Append(' ');
		_sb.Append(classes);
		return this;
	}

	/// <summary>
	/// Adds the given classes to the <c>CssBuilder</c> if the given condition is <see langword="true"/>
	/// </summary>
	/// <param name="classes">The classes to add</param>
	/// <param name="condition">The condition to check</param>
	/// <returns>The <c>CssBuilder</c></returns>
	public CssBuilder AddClasses(string classes, bool condition)
		=> condition ? AddClasses(classes) : this;

	/// <summary>
	/// Adds the developer-defined classes from a component's splatted attributes to the <c>CssBuilder</c>, if any
	/// </summary>
	/// <param name="attributes">The component's splatted attributes</param>
	/// <returns>The <c>CssBuilder</c></returns>
	public CssBuilder AddClasses(IReadOnlyDictionary<string, object>? attributes)
	{
		if (attributes?.TryGetValue("class", out var providedClasses) ?? false)
		{
			return AddClasses(providedClasses.ToString()!);
		}

		return this;
	}
}
