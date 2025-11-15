using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Sienar.Extensions;
using Sienar.Html;
using Sienar.Themes;

namespace Sienar.Ui;

/// <summary>
/// A button which can function as either a hyperlink or an action trigger
/// </summary>
public class Button : SienarComponentBase
{
	private bool IsLink => UserAttributes.ContainsKey("href");

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

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		Tag = IsLink ? "a" : "button";
		base.BuildRenderTree(builder);
	}

	/// <inheritdoc />
	protected override string? CreateCss()
		=> new CssBuilder()
			.AddClass("button")
			.AddClass($"button--{Color?.ToString().ToLower()}", Color is not null)
			.AddClass($"button--{Variant.ToString().ToLower()}")
			.AddClass((string?) UserAttributes.TryGetValue("class"))
			.Build();

	/// <inheritdoc />
	protected override Dictionary<string, object> CreateAttributes()
	{
		var attributes = new Dictionary<string, object>(UserAttributes);

		if (IsLink)
		{
			attributes.Add("role", "button");
		}

		return attributes;
	}
}

