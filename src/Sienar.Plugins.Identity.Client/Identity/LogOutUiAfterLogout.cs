namespace Sienar.Identity;

/// <summary>
/// Visually logs a user out after they have logged out of the application
/// </summary>
public class LogOutUiAfterLogout :
	UiLogoutManager,
	IAfterStatusAction<LogoutRequest>
{
	/// <summary>
	/// Creates a new instance of <c>LogOutUiAfterLogout</c>
	/// </summary>
	/// <param name="authProvider">The auth state provider</param>
	public LogOutUiAfterLogout(
		SienarAuthenticationStateProvider authProvider)
		: base(authProvider) {}

	/// <inheritdoc />
	public Task Handle(LogoutRequest input)
		=> Logout();
}
