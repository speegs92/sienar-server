using Sienar.Data;

namespace Sienar.Identity.Data;

/// <summary>
/// Maps from <see cref="SienarRole"/> to <see cref="RoleDto"/>
/// </summary>
public class RoleToDtoMapper : IMapper<SienarRole, RoleDto>
{
	/// <inheritdoc />
	public void Map(SienarRole source, RoleDto target)
	{
		target.Id = source.Id;
		target.Name = source.Name;
	}
}