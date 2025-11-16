using System;
using Microsoft.AspNetCore.Components;
using Sienar.Html;
using Sienar.Infrastructure;
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

	/// <summary>
	/// A delegate to invoke when the button is clicked
	/// </summary>
	/// <remarks>
	/// The provided delegate can have any method signature. The intention is to allow developers to inject dependencies into the delegate from the DI container. The developer can also request the <see cref="Microsoft.AspNetCore.Components.Web.MouseEventArgs"/>, which will be the instance of the event arguments provided by Blazor.
	/// </remarks>
	[Parameter]
	public Delegate? OnClick { get; set; }

	[Inject]
	private IDelegateHandler DelegateHandler { get; set; } = null!;
}
