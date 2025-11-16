using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Sienar.Extensions;
using Sienar.Html;

namespace Sienar.Ui;

/// <summary>
/// A dynamic HTML element
/// </summary>
public class DynamicElement : ComponentBase
{
	/// <summary>
	/// The HTML tag with which to render the component
	/// </summary>
	[Parameter]
	public string Tag { get; set; } = "div";

	/// <summary>
	/// Additional CSS classes to supply to the element, if any
	/// </summary>
	[Parameter]
	public string? Classes { get; set; }

	/// <summary>
	/// The child content with which to render the component
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// The developer-supplied attributes with which to render the component
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object> UserAttributes { get; set; } = new();

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		var cssBuilder = new CssBuilder()
			.AddClass(UserAttributes.TryGetValue("class")?.ToString())
			.AddClass(Classes);

		builder.OpenElement(0, Tag);
		builder.AddMultipleAttributes(1, UserAttributes);
		builder.AddAttribute(2, "class", cssBuilder.Build());
		builder.AddContent(3, ChildContent);
		builder.CloseElement();
	}
}
