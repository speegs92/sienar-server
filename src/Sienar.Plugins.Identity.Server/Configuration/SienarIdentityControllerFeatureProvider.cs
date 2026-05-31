using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Sienar.Configuration;

/// <summary>
/// Adds Sienar Identity generic controllers to the controller feature provider
/// </summary>
/// <typeparam name="T">The type of the user entity</typeparam>
public class SienarIdentityControllerFeatureProvider<T> : IApplicationFeatureProvider<ControllerFeature>
	where T : class, ISienarIdentityUser<T>, new()
{
	/// <inheritdoc />
	public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
	{
		feature.Controllers.Add(typeof(UsersController<T>).GetTypeInfo());
		feature.Controllers.Add(typeof(LockoutReasonController<T>).GetTypeInfo());
	}
}
