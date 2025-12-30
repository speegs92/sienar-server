using System.Security.Claims;

namespace Sienar.Identity;

/// <summary>
/// Creates user claims for use by the server application
/// </summary>
public class ServerUserClaimsFactory<T> : IUserClaimsFactory<T>
	where T : class, ISienarIdentityUser<T>
{
	/// <inheritdoc />
	public IEnumerable<Claim> CreateClaims(T user)
	{
		var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new(ClaimTypes.Name, user.Username),
			new(ClaimTypes.Email, user.Email)
		};

		if (user.Roles.Count > 0)
		{
			claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r)));
		}

		return claims;
	}
}
