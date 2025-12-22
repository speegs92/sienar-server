using System.Threading.Tasks;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <summary>
/// Ensures that the application has a valid CSRF token on startup by calling the CSRF endpoint in the Sienar REST API
/// </summary>
public class RefreshCsrfTokenOnStartup :
	CsrfTokenRefresher,
	IBeforeStatusAction<Startup>
{
	/// <summary>
	/// Creates a new instance of <c>InitializeCsrfTokenOnAppStartHook</c>
	/// </summary>
	/// <param name="client">The rest client</param>
	public RefreshCsrfTokenOnStartup(IRestClient client)
		: base(client) {}

	/// <inheritdoc />
	public Task Handle(Startup _)
		=> RefreshCsrfToken();
}