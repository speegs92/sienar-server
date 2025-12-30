namespace Sienar.Identity.Data;

/// <summary>
/// Maps a <see cref="ISienarIdentityUser{T}"/> to a <see cref="ViewUserDto"/>
/// </summary>
public class ViewUserMapper<T> : IMapper<T, ViewUserDto>
	where T : class, ISienarIdentityUser<T>
{
	/// <inheritdoc />
	public void Map(T source, ViewUserDto target)
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
