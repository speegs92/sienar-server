using Sienar.Html;

namespace Sienar.Ui;

/// <summary>
/// A container for Bootstrap-style <see cref="Column">columns</see>
/// </summary>
public class Row : SienarComponentBase
{
	/// <inheritdoc />
	protected override void AddCss(CssBuilder b)
		=> b.AddClass("row");
}
