using System.Security.Claims;

namespace Sienar.Identity;

public interface IUserClaimsFactory<T>
{
	IEnumerable<Claim> CreateClaims(T user);
}