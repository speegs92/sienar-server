using Sienar.Data;

namespace Sienar.Identity.Data;

/// <summary>
/// Maps a <see cref="SienarUser"/> to a <see cref="ViewUserDto"/>
/// </summary>
public class ViewUserMapper : IMapper<SienarUser, ViewUserDto>
{
	/// <inheritdoc />
	public void Map(SienarUser source, ViewUserDto target)
	{
		target.Id = source.Id;
		target.ConcurrencyStamp = source.ConcurrencyStamp;
		target.Username = source.Username;
		target.Email = source.Email;
		target.EmailConfirmed = source.EmailConfirmed;
		target.LockoutEnd = source.LockoutEnd;
		target.Roles = source.Roles;
	}
}
