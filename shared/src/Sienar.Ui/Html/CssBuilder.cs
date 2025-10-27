using System.Collections.Generic;

namespace Sienar.Html;

/// <summary>
/// Container for CSS classes that can be appended to and built
/// </summary>
public class CssBuilder
{
	private readonly List<string> _classes = [];

	/// <summary>
	/// Adds a CSS class to the <c>CssBuilder</c>
	/// </summary>
	/// <param name="className">The CSS class to add</param>
	/// <returns>the <c>CssBuilder</c></returns>
	public CssBuilder AddClass(string? className)
	{
		if (!string.IsNullOrEmpty(className))
		{
			_classes.Add(className);
		}

		return this;
	}

	/// <summary>
	/// Adds a CSS class to the <c>CssBuilder</c> if a condition is true
	/// </summary>
	/// <param name="className">The CSS class to add</param>
	/// <param name="flag">The condition to check</param>
	/// <returns>the <c>CssBuilder</c></returns>
	public CssBuilder AddClass(string? className, bool flag)
		=> flag ? AddClass(className) : this;

	/// <summary>
	/// Builds the final CSS string
	/// </summary>
	/// <returns>the CSS string, if any</returns>
	public string? Build()
	{
		if (_classes.Count == 0)
		{
			return null;
		}

		var built = string.Join(' ', _classes);
		_classes.Clear();
		return built;
	}
}
