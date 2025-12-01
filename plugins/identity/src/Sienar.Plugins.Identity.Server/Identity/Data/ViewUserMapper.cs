using Sienar.Data;

namespace Sienar.Identity.Data;

/// <summary>
/// Maps a <see cref="SienarUser"/> to a <see cref="ViewUserDto"/>
/// </summary>
public class ViewUserMapper : IMapper<SienarUser, ViewUserDto>
{
	private readonly IMapper<SienarRole, RoleDto> _roleMapper;

	/// <summary>
	/// Creates a new instance of <c>ViewUserMapper</c>
	/// </summary>
	/// <param name="roleMapper">The role mapper</param>
	public ViewUserMapper(IMapper<SienarRole, RoleDto> roleMapper)
	{
		_roleMapper = roleMapper;
	}

	/// <inheritdoc />
	public void Map(SienarUser source, ViewUserDto target)
	{
		target.Id = source.Id;
		target.ConcurrencyStamp = source.ConcurrencyStamp;
		target.Username = source.Username;
		target.Email = source.Email;
		target.EmailConfirmed = source.EmailConfirmed;
		target.LockoutEnd = source.LockoutEnd;

		foreach (var role in source.Roles)
		{
			var roleDto = new RoleDto();
			_roleMapper.Map(role, roleDto);
			target.Roles.Add(roleDto);
		}
	}
}
