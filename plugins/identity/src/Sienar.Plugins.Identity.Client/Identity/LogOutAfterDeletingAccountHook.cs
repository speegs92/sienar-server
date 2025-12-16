namespace Sienar.Identity;

/// <summary>
/// Logs a user out after they have deleted their account
/// </summary>
public class LogOutAfterDeletingAccountHook :
	IAfterStatusAction<DeleteAccountRequest>
{
	private readonly SienarAuthenticationStateProvider _authProvider;

	/// <summary>
	/// Creates a new instance of <c>LogOutAfterDeletingAccountHook</c>
	/// </summary>
	/// <param name="authProvider">The auth state provider</param>
	public LogOutAfterDeletingAccountHook(
		SienarAuthenticationStateProvider authProvider)
	{
		_authProvider = authProvider;
	}

	/// <inheritdoc />
	public Task Handle(DeleteAccountRequest input)
	{
		_authProvider.NotifyUserAuthentication([], false);
		return Task.CompletedTask;
	}
}