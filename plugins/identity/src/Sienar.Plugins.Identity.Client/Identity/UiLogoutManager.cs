namespace Sienar.Identity;

/// <summary>
/// Logs a user out of the UI
/// </summary>
public class UiLogoutManager
{
	private readonly SienarAuthenticationStateProvider _authProvider;

	/// <summary>
	/// Creates a new instance of <c>UiLogoutManager</c>
	/// </summary>
	/// <param name="authProvider">The auth state provider</param>
	protected UiLogoutManager(
		SienarAuthenticationStateProvider authProvider)
		=> _authProvider = authProvider;

	/// <summary>
	/// Logs the user out of the UI
	/// </summary>
	protected Task Logout()
	{
		_authProvider.Logout();
		return Task.CompletedTask;
	}
}
