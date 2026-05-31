namespace Sienar.Layouts;

/// <summary>
/// A standard base layout for use with Sienar
/// </summary>
public partial class DashboardLayoutBase
{
	/// <summary>
	/// The menu names to render with this layout
	/// </summary>
	protected string[] MenuNames = [];

	/// <summary>
	/// The type of the current layout
	/// </summary>
	protected Type LayoutType = null!;
}