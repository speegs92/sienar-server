#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
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
	public Task<IActionResult> Read(
		[FromQuery] Filter? filter,
		[FromServices] IReadAllActionOrchestrator<ViewUserDto, SienarUser> orchestrator)
		=> orchestrator.Execute(filter);

	[HttpGet("{id:int}")]
	public Task<IActionResult> Read(
		int id,
		[FromQuery] Filter? filter,
		[FromServices] IReadActionOrchestrator<ViewUserDto, SienarUser> orchestrator)
		=> orchestrator.Execute(id, filter);

	[HttpPost]
	public Task<IActionResult> Create(
		UpsertUserDto user,
		[FromServices] ICreateActionOrchestrator<UpsertUserDto, SienarUser> orchestrator)
		=> orchestrator.Execute(user);

	[HttpPut]
	public Task<IActionResult> Update(
		UpsertUserDto user,
		[FromServices] IUpdateActionOrchestrator<UpsertUserDto, SienarUser> orchestrator)
		=> orchestrator.Execute(user);

	[HttpDelete("{id:int}")]
	public Task<IActionResult> Delete(
		int id,
		[FromServices] IDeleteActionOrchestrator<SienarUser> orchestrator)
		=> orchestrator.Execute(id);

	[HttpPost("roles")]
	public Task<IActionResult> AddToRole(
		AddUserToRoleRequest data,
		[FromServices] IStatusActionOrchestrator<AddUserToRoleRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpDelete("roles")]
	public Task<IActionResult> RemoveFromRole(
		RemoveUserFromRoleRequest data,
		[FromServices] IStatusActionOrchestrator<RemoveUserFromRoleRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPatch("lock")]
	public Task<IActionResult> LockUser(
		LockUserAccountRequest data,
		[FromServices] IStatusActionOrchestrator<LockUserAccountRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpDelete("lock")]
	public Task<IActionResult> UnlockUser(
		UnlockUserAccountRequest data,
		[FromServices] IStatusActionOrchestrator<UnlockUserAccountRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPatch("confirm")]
	public Task<IActionResult> ConfirmUserAccount(
		ManuallyConfirmUserAccountRequest data,
		[FromServices] IStatusActionOrchestrator<ManuallyConfirmUserAccountRequest> orchestrator)
		=> orchestrator.Execute(data);
}