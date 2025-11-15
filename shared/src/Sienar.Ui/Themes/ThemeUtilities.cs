using System;

namespace Sienar.Themes;

/// <summary>
/// Contains methods used to work with Sienar theme constructs
/// </summary>
public static class ThemeUtilities
{
	/// <summary>
	/// Converts an enum name into a lowercase string
	/// </summary>
	/// <param name="e">The enum value</param>
	/// <returns></returns>
	public static string? GetCssString(Enum? e)
		=> e?.ToString().ToLower();
}
