using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Sienar.Identity;

public class UserClaimsPrincipalFactory<T>
	: IUserClaimsPrincipalFactory<T>
	where T : class, ISienarIdentityUser<T>
{
	private readonly IUserClaimsFactory<T> _claimsFactory;

	public UserClaimsPrincipalFactory(IUserClaimsFactory<T> claimsFactory)
	{
		_claimsFactory = claimsFactory;
	}

	public async Task<ClaimsPrincipal> CreateAsync(T user)
	{
		var identity = await GenerateClaims(user);
		return new ClaimsPrincipal(identity);
	}

	private Task<ClaimsIdentity> GenerateClaims(T user)
	{
		var identity = new ClaimsIdentity(
			_claimsFactory.CreateClaims(user),
			CookieAuthenticationDefaults.AuthenticationScheme);
		return Task.FromResult(identity);
	}
}