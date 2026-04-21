using Microsoft.AspNetCore.Components;
using Sienar.Html;
using Sienar.Themes;

namespace Sienar.Ui;

public partial class Card
{
	private string? Css => new CssBuilder()
		.AddClass("card")
		.AddClass($"card--{ThemeUtilities.GetCssString(Color)}", Color is not null)
		.AddClass($"card--{ThemeUtilities.GetCssString(Variant)}", Color is not null) // If there's no color, the variant doesn't do anything
		.Build();

	[Parameter]
	public Color? Color { get; set; }

	[Parameter]
	public Variant Variant { get; set; } = Variant.Solid;

	public Card() => Tag = "article";
}
