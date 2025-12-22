using Microsoft.AspNetCore.Identity;

namespace Sienar.Identity.Data;

/// <summary>
/// Maps a <see cref="UpsertUserDto"/> to a <see cref="SienarUser"/>
/// </summary>
public class UpsertUserMapper : IMapper<UpsertUserDto, SienarUser>
{
	private readonly IPasswordHasher<SienarUser> _passwordHasher;

	/// <summary>
	/// Creates a new instance of <c>UpsertUserMapper</c>
	/// </summary>
	/// <param name="passwordHasher">The password hasher</param>
	public UpsertUserMapper(IPasswordHasher<SienarUser> passwordHasher)
	{
		_passwordHasher = passwordHasher;
	}

	/// <inheritdoc />
	public void Map(UpsertUserDto source, SienarUser target)
	{
		target.Username = source.Username;
		target.NormalizedUsername = source.Username.ToNormalized();
		target.Email = source.Email;
		target.NormalizedEmail = source.Email.ToNormalized();
		target.ConcurrencyStamp = source.ConcurrencyStamp;
		target.Roles = source.Roles;

		if (!string.IsNullOrEmpty(source.Password))
		{
			target.PasswordHash = _passwordHasher.HashPassword(
				target,
				source.Password);
		}
	}
}
