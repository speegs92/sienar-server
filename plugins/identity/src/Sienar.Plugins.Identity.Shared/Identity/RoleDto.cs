using Sienar.Data;

namespace Sienar.Identity;

/// <summary>
/// An application role
/// </summary>
[EntityName(Singular = "role", Plural ="roles")]
[RestEndpoint("roles")]
public class RoleDto : EntityBase
{
	/// <summary>
	/// The name of the role
	/// </summary>
	public string Name { get; set; } = string.Empty;
}