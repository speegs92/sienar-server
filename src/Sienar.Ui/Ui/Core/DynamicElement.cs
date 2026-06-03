// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

/// <summary>
/// A minimally configurable component which renders as a dynamic HTML element
/// </summary>
public class DynamicElement : ComponentBase
{
	/// <summary>
	/// The HTML tag with which to render the underlying HTML element
	/// </summary>
	[Parameter]
	public string? Tag { get; set; }

	/// <summary>
	/// The HTML tag with which to render the underlying element if no other tag is provided
	/// </summary>
	[Parameter]
	public string DefaultTag { get; set; } = "div";

	/// <summary>
	/// The developer-provided attributes for the underlying HTML element
	/// </summary>
	[Parameter]
	public IReadOnlyDictionary<string, object>? Attributes { get; set; }

	/// <summary>
	/// The child content with which to render the underlying HTML element
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenElement(0, Tag ?? DefaultTag);
		builder.AddMultipleAttributes(1, Attributes);
		builder.AddContent(2, ChildContent);
		builder.CloseElement();
	}
}
