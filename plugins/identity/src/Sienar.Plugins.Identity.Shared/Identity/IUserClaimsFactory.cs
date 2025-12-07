using System.Security.Claims;

namespace Sienar.Identity;

public interface IUserClaimsFactory
{
	IEnumerable<Claim> CreateClaims(SienarUser user);
}