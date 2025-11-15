using Microsoft.AspNetCore.Components;
using Sienar.Html;
using Sienar.Themes;

namespace Sienar.Ui;

/// <summary>
/// A generic HTML container which accepts a maximum width and an alignment
/// </summary>
public class Container : SienarComponentBase
{
	/// <summary>
	/// The alignment of the container within its parent
	/// </summary>
	[Parameter]
	public Alignment Alignment { get; set; } = Alignment.Center;

	/// <summary>
	/// The maximum width of the container
	/// </summary>
	/// <remarks>
	/// If not set, the container will behave like a standard block-level HTML element.
	/// </remarks>
	[Parameter]
	public Breakpoint? MaxWidth { get; set; }

	/// <inheritdoc />
	protected override string? CreateCss()
		=> new CssBuilder()
			.AddClass("container")
			.AddClass($"container--{Alignment.ToString().ToLower()}")
			.AddClass($"container--{MaxWidth?.ToString().ToLower()}", MaxWidth is not null)
			.Build();
}
