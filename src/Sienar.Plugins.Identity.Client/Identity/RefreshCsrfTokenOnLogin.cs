namespace Sienar.Identity;

/// <summary>
/// Ensures that the application refreshes the CSRF token after the user logs in
/// </summary>
public class RefreshCsrfTokenOnLogin :
	CsrfTokenRefresher,
	IAfterGeneralAction<LoginRequest>
{
	/// <summary>
	/// Creates a new instance of <c>RefreshCsrfTokenOnLogin</c>
	/// </summary>
	/// <param name="client">The rest client</param>
	public RefreshCsrfTokenOnLogin(IRestClient client)
		: base(client) {}

	/// <inheritdoc />
	public Task Handle(LoginRequest _)
		=> RefreshCsrfToken();
}
