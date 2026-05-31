namespace Sienar.Infrastructure;

/// <summary>
/// Refreshes the application CSRF token
/// </summary>
public abstract class CsrfTokenRefresher
{
	private readonly IRestClient _client;

	/// <summary>
	/// Creates a new isntance of <c>CsrfTokenRefresher</c>
	/// </summary>
	/// <param name="client">The rest client</param>
	protected CsrfTokenRefresher(IRestClient client)
	{
		_client = client;
	}

	/// <summary>
	/// Refreshes the app's CSRF token
	/// </summary>
	protected Task RefreshCsrfToken()
		=> _client.RefreshCsrfToken();
}
