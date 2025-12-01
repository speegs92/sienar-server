using System.Collections.Generic;
using Sienar.Data;

namespace Sienar.Identity;

[EntityName(Singular = "role", Plural ="roles")]
public class SienarRole : EntityBase
{
	/// <summary>
	/// The name of the role
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// The normalized name of the role
	/// </summary>
	public string NormalizedName { get; set; } = string.Empty;

	/// <summary>
	/// A list of all users in this role
	/// </summary>
	public List<SienarUser> Users { get; set; } = [];

	/// <inheritdoc/>
	public override string ToString() => Name;
}