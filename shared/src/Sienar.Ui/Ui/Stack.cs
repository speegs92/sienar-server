using Microsoft.AspNetCore.Components;
using Sienar.Html;
using Sienar.Themes;

namespace Sienar.Ui;

/// <summary>
/// A Flexbox utility class which enables developers to easily align content vertically and horizontally into stacks of columns and rows
/// </summary>
public class Stack : SienarComponentBase
{
	/// <summary>
	/// The stack direction
	/// </summary>
	[Parameter]
	public FlexDirection Direction { get; set; } = FlexDirection.Horizontal;

	/// <summary>
	/// The stack content justification
	/// </summary>
	[Parameter]
	public FlexJustify Justify { get; set; }

	/// <summary>
	/// The stack item alignment
	/// </summary>
	[Parameter]
	public FlexAlign Align { get; set; }

	/// <inheritdoc />
	protected override void AddCss(CssBuilder b)
		=> b
			.AddClass("d-flex")
			.AddClass("flex-wrap")
			.AddClass($"justify-content-{ThemeUtilities.GetCssString(Justify)}")
			.AddClass($"align-items-{ThemeUtilities.GetCssString(Align)}")
			.AddClass("flex-row", Direction == FlexDirection.Horizontal)
			.AddClass("flex-column", Direction == FlexDirection.Vertical);
}
