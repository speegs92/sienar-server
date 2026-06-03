// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

/// <summary>
/// A base Bulma element
/// </summary>
public abstract class BulmaElement : ComponentBase
{
	/// <summary>
	/// The HTML tag with which to render the Bulma element
	/// </summary>
	[Parameter]
	public string? Tag { get; set; }

	/// <summary>
	/// The HTML attributes to add to the rendered HTML
	/// </summary>
	[Parameter]
	public IReadOnlyDictionary<string, object>? Attributes { get; set; }
}
