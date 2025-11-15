using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Sienar.Extensions;
using Sienar.Html;

namespace Sienar.Ui;

/// <summary>
/// A base class for Sienar components
/// </summary>
public abstract class SienarComponentBase : ComponentBase
{
	/// <summary>
	/// The HTML tag with which to render the component
	/// </summary>
	[Parameter]
	public string Tag { get; set; } = "div";

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
		var cssBuilder = new CssBuilder();
		cssBuilder.AddClass(UserAttributes.TryGetValue("class")?.ToString());
		AddCss(cssBuilder);

		builder.OpenElement(0, Tag);
		builder.AddMultipleAttributes(1, CreateAttributes());
		builder.AddAttribute(2, "class", cssBuilder.Build());
		builder.AddContent(3, ChildContent);
		builder.OpenRegion(4);
		ModifyRenderTree(builder);
		builder.CloseRegion();
		builder.CloseElement();
	}

	/// <summary>
	/// A method to create CSS class names for the underlying HTML element
	/// </summary>
	/// <returns>The CSS class names to render on the element</returns>
	protected virtual void AddCss(CssBuilder b) {}

	/// <summary>
	/// A method to create HTML attributes for the underlying HTML element
	/// </summary>
	/// <returns>The HTML attributes to render on the element</returns>
	protected virtual Dictionary<string, object> CreateAttributes() => UserAttributes;

	/// <summary>
	/// A method to perform additional operations on the <see cref="RenderTreeBuilder"/>
	/// </summary>
	/// <param name="builder">The <see cref="RenderTreeBuilder"/></param>
	protected virtual void ModifyRenderTree(RenderTreeBuilder builder) {}
}
