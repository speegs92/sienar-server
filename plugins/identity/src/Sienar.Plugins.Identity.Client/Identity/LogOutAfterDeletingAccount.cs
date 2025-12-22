namespace Sienar.Identity;

/// <summary>
/// Logs a user out after they have deleted their account
/// </summary>
public class LogOutAfterDeletingAccount :
	UiLogoutManager,
	IAfterStatusAction<DeleteAccountRequest>
{
	/// <summary>
	/// Creates a new instance of <c>LogOutAfterDeletingAccount</c>
	/// </summary>
	/// <param name="authProvider">The auth state provider</param>
	public LogOutAfterDeletingAccount(
		SienarAuthenticationStateProvider authProvider)
		: base(authProvider) {}

	/// <inheritdoc />
	public Task Handle(DeleteAccountRequest input)
		=> Logout();
}