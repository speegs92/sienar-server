namespace Sienar.Infrastructure;

/// <summary>
/// The menus supported by Sienar out of the box
/// </summary>
public static class IdentityMenus
{
	/// <summary>
	/// The main dashboard menu, rendered in the side menu
	/// </summary>
	public const string Main = "sienar-identity-main";

	/// <summary>
	/// The user settings menu, rendered by the user badge in the footer of the side menu
	/// </summary>
	public const string UserSettings = "sienar-identity-user-settings";

	/// <summary>
	/// The user logout menu, rendered by the user badge in the footer of the side menu
	/// </summary>
	public const string UserLogout = "sienar-identity-user-logout";

	/// <summary>
	/// The info menu, rendered in the side menu
	/// </summary>
	public const string Info = "sienar-identity-info";

	/// <summary>
	/// The user management menu, rendered on the default dashboard homepage
	/// </summary>
	public const string UserManagement = "sienar-identity-user-management";
}