using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Sienar.Extensions;
using Sienar.Html;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

/// <summary>
/// A button which can function as either a hyperlink or an action trigger
/// </summary>
public class Button : ComponentBase
{
	private string? Css => new CssBuilder()
		.AddClass("button")
		.AddClass($"button--{Color?.ToString().ToLower()}", Color is not null)
		.AddClass($"button--{Variant.ToString().ToLower()}")
		.AddClass((string?) UserAttributes.TryGetValue("class"))
		.Build();

	/// <summary>
	/// The theme color of the button
	/// </summary>
	[Parameter]
	public ThemeColor? Color { get; set; } = ThemeColor.Primary;

	/// <summary>
	/// The variant of the button
	/// </summary>
	[Parameter]
	public ThemeVariant Variant { get; set; }

	/// <summary>
	/// The child content of the button
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// The user-supplied element attributes
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object> UserAttributes { get; set; } = new();

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		var isLink = UserAttributes.ContainsKey("href");
		var tag = isLink ? "a" : "button";

		builder.OpenElement(0, tag);
		builder.AddMultipleAttributes(1, UserAttributes);
		builder.AddAttribute(2, "class", Css);

		if (!isLink)
		{
			builder.AddAttribute(3, "role", "button");
		}

		builder.AddContent(4, ChildContent);

		builder.CloseElement();
	}
}

