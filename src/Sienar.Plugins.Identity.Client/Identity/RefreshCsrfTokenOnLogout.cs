namespace Sienar.Identity;

/// <summary>
/// Ensures that the application refreshes the CSRF token after the user logs out
/// </summary>
public class RefreshCsrfTokenOnLogout :
	CsrfTokenRefresher,
	IAfterStatusAction<LogoutRequest>
{
	/// <summary>
	/// Creates a new instance of <c>RefreshCsrfTokenOnLogout</c>
	/// </summary>
	/// <param name="client">The rest client</param>
	public RefreshCsrfTokenOnLogout(IRestClient client)
		: base(client) {}

	/// <inheritdoc />
	public Task Handle(LogoutRequest _)
		=> RefreshCsrfToken();
}
