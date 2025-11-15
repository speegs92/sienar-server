using Microsoft.AspNetCore.Components;
using Sienar.Html;

namespace Sienar.Ui;

/// <summary>
/// A Bootstrap-style column component with support for 12 columns and breakpoint-specific column sizes
/// </summary>
public class Column : SienarComponentBase
{
	/// <summary>
	/// The column size for the smallest screens
	/// </summary>
	[Parameter]
	public int Col { get; set; }

	/// <summary>
	/// The column size for small screens
	/// </summary>
	[Parameter]
	public int? Sm { get; set; }

	/// <summary>
	/// The column size for medium screens
	/// </summary>
	[Parameter]
	public int? Md { get; set; }

	/// <summary>
	/// The column size for large screens
	/// </summary>
	[Parameter]
	public int? Lg { get; set; }

	/// <summary>
	/// The column size for extra-large screens
	/// </summary>
	[Parameter]
	public int? Xl { get; set; }

	/// <summary>
	/// The column size for extra-extra-large screens
	/// </summary>
	[Parameter]
	public int? Xxl { get; set; }

	/// <inheritdoc />
	protected override void AddCss(CssBuilder b)
		=> b
			.AddClass($"col-{Col}")
			.AddClass($"col-sm-{Sm}", Sm.HasValue)
			.AddClass($"col-md-{Md}", Md.HasValue)
			.AddClass($"col-lg-{Lg}", Lg.HasValue)
			.AddClass($"col-xl-{Xl}", Xl.HasValue)
			.AddClass($"col-xxl-{Xxl}", Xxl.HasValue);
}
