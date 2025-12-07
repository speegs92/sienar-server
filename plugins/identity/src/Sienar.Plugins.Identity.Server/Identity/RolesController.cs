#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1822 // Member can be marked as static

using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace Sienar.Identity;

/// <exclude />
[ApiController]
[Route("/api/roles")]
[Authorize(Roles = Roles.Admin)]
public class RolesController
{
	[HttpGet]
	[UsedImplicitly]
	public Task<IActionResult> Read(
		[FromServices] IReadAllActionOrchestrator<RoleDto, SienarRole> orchestrator)
		=> orchestrator.Execute(Filter.GetAll());
}