using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

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
}
