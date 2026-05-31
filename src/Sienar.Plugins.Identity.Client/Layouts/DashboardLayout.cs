namespace Sienar.Layouts;

public class DashboardLayout : DashboardLayoutBase
{
	public DashboardLayout()
	{
		MenuNames = [
			IdentityMenus.Main,
			IdentityMenus.Info
		];
		LayoutType = typeof(DashboardLayout);
	}
}