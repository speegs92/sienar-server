using Microsoft.AspNetCore.Identity;

namespace Sienar.Identity.Data;

/// <summary>
/// Maps a <see cref="UpsertUserDto"/> to a <see cref="ISienarIdentityUser{T}"/>
/// </summary>
public class UpsertUserMapper<T> : IMapper<UpsertUserDto, T>
	where T : class, ISienarIdentityUser<T>
{
	private readonly IPasswordHasher<T> _passwordHasher;

	/// <summary>
	/// Creates a new instance of <c>UpsertUserMapper</c>
	/// </summary>
	/// <param name="passwordHasher">The password hasher</param>
	public UpsertUserMapper(IPasswordHasher<T> passwordHasher)
	{
		_passwordHasher = passwordHasher;
	}

	/// <inheritdoc />
	public void Map(UpsertUserDto source, T target)
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
