using Sienar.Data;
using Sienar.Extensions;

namespace Sienar.Identity.Data;

/// <summary>
/// Maps a <see cref="UpsertUserDto"/> to a <see cref="SienarUser"/>
/// </summary>
public class UpsertUserMapper : IMapper<UpsertUserDto, SienarUser> 
{
	/// <inheritdoc />
	public void Map(UpsertUserDto source, SienarUser target)
	{
		target.Username = source.Username;
		target.NormalizedUsername = source.Username.ToNormalized();
		target.Email = source.Email;
		target.NormalizedEmail = source.Email.ToNormalized();
		target.Password = source.Password; // This is just a mapper. Hashing is beyond its scope
		target.ConcurrencyStamp = source.ConcurrencyStamp;
	}
}
