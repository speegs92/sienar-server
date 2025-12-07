#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1822 // Member can be marked as static

using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;

namespace Sienar.Identity;

/// <exclude />
[ApiController]
[Route("/api/users")]
[Authorize(Roles = Roles.Admin)]
public class UsersController : SienarController
{
	public UsersController(IOperationResultMapper mapper)
		: base(mapper) {}

	[HttpGet]
	[UsedImplicitly]
	public Task<IActionResult> Read(
		[FromQuery] Filter? filter,
		[FromServices] IReadAllActionOrchestrator<ViewUserDto, SienarUser> orchestrator)
		=> orchestrator.Execute(filter);

	[HttpGet("{id:int}")]
	[UsedImplicitly]
	public Task<IActionResult> Read(
		int id,
		[FromQuery] Filter? filter,
		[FromServices] IReadActionOrchestrator<ViewUserDto, SienarUser> orchestrator)
		=> orchestrator.Execute(id, filter);

	[HttpPost]
	[UsedImplicitly]
	public Task<IActionResult> Create(
		UpsertUserDto user,
		[FromServices] ICreateActionOrchestrator<UpsertUserDto, SienarUser> orchestrator)
		=> orchestrator.Execute(user);

	[HttpPut]
	[UsedImplicitly]
	public Task<IActionResult> Update(
		UpsertUserDto user,
		[FromServices] IUpdateActionOrchestrator<UpsertUserDto, SienarUser> orchestrator)
		=> orchestrator.Execute(user);

	[HttpDelete("{id:int}")]
	[UsedImplicitly]
	public Task<IActionResult> Delete(
		int id,
		[FromServices] IDeleteActionOrchestrator<SienarUser> orchestrator)
		=> orchestrator.Execute(id);

	[HttpPost("roles")]
	[UsedImplicitly]
	public Task<IActionResult> AddToRole(
		AddUserToRoleRequest data,
		[FromServices] IStatusActionOrchestrator<AddUserToRoleRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpDelete("roles")]
	[UsedImplicitly]
	public Task<IActionResult> RemoveFromRole(
		RemoveUserFromRoleRequest data,
		[FromServices] IStatusActionOrchestrator<RemoveUserFromRoleRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPatch("lock")]
	[UsedImplicitly]
	public Task<IActionResult> LockUser(
		LockUserAccountRequest data,
		[FromServices] IStatusActionOrchestrator<LockUserAccountRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpDelete("lock")]
	[UsedImplicitly]
	public Task<IActionResult> UnlockUser(
		UnlockUserAccountRequest data,
		[FromServices] IStatusActionOrchestrator<UnlockUserAccountRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPatch("confirm")]
	[UsedImplicitly]
	public Task<IActionResult> ConfirmUserAccount(
		ManuallyConfirmUserAccountRequest data,
		[FromServices] IStatusActionOrchestrator<ManuallyConfirmUserAccountRequest> orchestrator)
		=> orchestrator.Execute(data);
}