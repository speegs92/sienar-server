#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1822 // Member can be marked as static

using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace Sienar.Identity;

/// <exclude />
[ApiController]
[Route("/api/users")]
[Authorize(Roles = Roles.Admin)]
public class UsersController<T>
	where T : class, ISienarIdentityUser<T>, new()
{
	[HttpGet]
	[UsedImplicitly]
	public Task<IActionResult> Read(
		[FromQuery] Filter? filter,
		[FromServices] IReadAllActionOrchestrator<ViewUserDto, T> orchestrator)
		=> orchestrator.Execute(filter);

	[HttpGet("{id:int}")]
	[UsedImplicitly]
	public Task<IActionResult> Read(
		int id,
		[FromQuery] Filter? filter,
		[FromServices] IReadActionOrchestrator<ViewUserDto, T> orchestrator)
		=> orchestrator.Execute(id, filter);

	[HttpPost]
	[UsedImplicitly]
	public Task<IActionResult> Create(
		[FromForm] UpsertUserDto user,
		[FromServices] ICreateActionOrchestrator<UpsertUserDto, T> orchestrator)
		=> orchestrator.Execute(user);

	[HttpPut]
	[UsedImplicitly]
	public Task<IActionResult> Update(
		[FromForm] UpsertUserDto user,
		[FromServices] IUpdateActionOrchestrator<UpsertUserDto, T> orchestrator)
		=> orchestrator.Execute(user);

	[HttpDelete("{id:int}")]
	[UsedImplicitly]
	public Task<IActionResult> Delete(
		int id,
		[FromServices] IDeleteActionOrchestrator<T> orchestrator)
		=> orchestrator.Execute(id);

	[HttpPost("roles")]
	[UsedImplicitly]
	public Task<IActionResult> AddToRole(
		[FromForm] AddUserToRoleRequest data,
		[FromServices] IStatusActionOrchestrator<AddUserToRoleRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpDelete("roles")]
	[UsedImplicitly]
	public Task<IActionResult> RemoveFromRole(
		[FromForm] RemoveUserFromRoleRequest data,
		[FromServices] IStatusActionOrchestrator<RemoveUserFromRoleRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPatch("lock")]
	[UsedImplicitly]
	public Task<IActionResult> LockUser(
		[FromForm] LockUserAccountRequest data,
		[FromServices] IStatusActionOrchestrator<LockUserAccountRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpDelete("lock")]
	[UsedImplicitly]
	public Task<IActionResult> UnlockUser(
		[FromForm] UnlockUserAccountRequest data,
		[FromServices] IStatusActionOrchestrator<UnlockUserAccountRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPatch("confirm")]
	[UsedImplicitly]
	public Task<IActionResult> ConfirmUserAccount(
		[FromForm] ManuallyConfirmUserAccountRequest data,
		[FromServices] IStatusActionOrchestrator<ManuallyConfirmUserAccountRequest> orchestrator)
		=> orchestrator.Execute(data);
}