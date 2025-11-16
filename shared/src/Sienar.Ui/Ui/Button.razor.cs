using Microsoft.AspNetCore.Components;
using Sienar.Html;
using Sienar.Themes;

namespace Sienar.Ui;

/// <summary>
/// A button which can function as either a hyperlink or an action trigger
/// </summary>
public partial class Button
{
	private bool IsLink => UserAttributes.ContainsKey("href");

	private string? Css => new CssBuilder()
		.AddClass("button")
		.AddClass($"button--{ThemeUtilities.GetCssString(Color)}", Color is not null)
		.AddClass($"button--{ThemeUtilities.GetCssString(Variant)}")
		.Build();

	/// <summary>
	/// The theme color of the button
	/// </summary>
	[Parameter]
	public Color? Color { get; set; } = Themes.Color.Primary;

	/// <summary>
	/// The variant of the button
	/// </summary>
	[Parameter]
	public Variant Variant { get; set; }
}
