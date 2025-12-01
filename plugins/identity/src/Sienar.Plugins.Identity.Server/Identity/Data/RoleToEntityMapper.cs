using System;
using Sienar.Data;

namespace Sienar.Identity.Data;

/// <summary>
/// Maps from <see cref="RoleDto"/> to <see cref="SienarRole"/>
/// </summary>
public class RoleToEntityMapper : IMapper<RoleDto, SienarRole>
{
	/// <inheritdoc />
	public void Map(RoleDto source, SienarRole target)
	{
		throw new InvalidOperationException("Sienar does not support creating or updating roles from the user interface. Please ask your app developer to create new roles for your use.");
	}
}