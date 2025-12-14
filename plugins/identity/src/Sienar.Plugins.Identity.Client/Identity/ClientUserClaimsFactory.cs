using System.Security.Claims;

namespace Sienar.Identity;

/// <summary>
/// Creates user claims for use by the client application
/// </summary>
public class ClientUserClaimsFactory : IUserClaimsFactory<ViewUserDto>
{
	/// <inheritdoc />
	public IEnumerable<Claim> CreateClaims(ViewUserDto user)
	{
		var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new(ClaimTypes.Name, user.Username),
			new(ClaimTypes.Email, user.Email)
		};

		if (user.Roles.Count > 0)
		{
			claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r.Name)));
		}

		return claims;
	}
}