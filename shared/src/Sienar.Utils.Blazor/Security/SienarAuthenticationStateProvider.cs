using System.Security.Claims;

namespace Sienar.Security;

/// <summary>
/// A WASM-friendly implementation of <see cref="AuthenticationStateProvider"/>
/// </summary>
public class SienarAuthenticationStateProvider : AuthenticationStateProvider
{
	private AuthenticationState _authState = CreateAuthenticationState([], false);

	/// <ignore />
	public override Task<AuthenticationState> GetAuthenticationStateAsync()
		=> Task.FromResult(_authState);

	/// <summary>
	/// Logs a user in to the UI
	/// </summary>
	/// <param name="claims">The user's claims</param>
	public void Login(IEnumerable<Claim> claims)
	{
		_authState = CreateAuthenticationState(claims, true);
		NotifyAuthenticationStateChanged(Task.FromResult(_authState));
	}

	/// <summary>
	/// Logs a user out from the UI
	/// </summary>
	public void Logout()
	{
		_authState = CreateAuthenticationState([], false);
		NotifyAuthenticationStateChanged(Task.FromResult(_authState));
	}

	private static AuthenticationState CreateAuthenticationState(
		IEnumerable<Claim> claims,
		bool isAuthenticated)
	{
		var identity = isAuthenticated
			? new ClaimsIdentity(claims, "CookieAuth")
			: new ClaimsIdentity(claims);
		var principal = new ClaimsPrincipal(identity);
		return new AuthenticationState(principal);
	}
}
