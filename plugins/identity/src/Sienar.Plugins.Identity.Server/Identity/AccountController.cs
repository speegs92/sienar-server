#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Identity.Results;
using Sienar.Infrastructure;

namespace Sienar.Identity;

/// <exclude />
[ApiController]
[Route("/api/account")]
[Authorize]
public class AccountController : SienarController
{
	public AccountController(IOperationResultMapper mapper)
		: base(mapper) {}

	[HttpPost]
	[AllowAnonymous]
	public Task<IActionResult> Register(
		RegisterRequest data,
		[FromServices] IStatusActionOrchestrator<RegisterRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpGet]
	public Task<IActionResult> GetAccountData(
		[FromServices] IResultActor<AccountDataResult> actor)
		=> Execute(actor.Execute);

	[HttpDelete]
	public Task<IActionResult> DeleteAccount(
		DeleteAccountRequest data,
		[FromServices] IStatusActionOrchestrator<DeleteAccountRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPost("confirm")]
	[AllowAnonymous]
	public Task<IActionResult> Confirm(
		ConfirmAccountRequest data,
		[FromServices] IStatusActionOrchestrator<ConfirmAccountRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPost("login")]
	[AllowAnonymous]
	public Task<IActionResult> Login(
		LoginRequest data,
		[FromServices] IGeneralActor<LoginRequest, LoginResult> service)
		=> Execute(() => service.Execute(data));

	[HttpDelete("login")]
	public Task<IActionResult> Logout(
		LogoutRequest data,
		[FromServices] IStatusActionOrchestrator<LogoutRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpDelete("password")]
	[AllowAnonymous]
	public Task<IActionResult> RequestPasswordReset(
		ForgotPasswordRequest data,
		[FromServices] IStatusActionOrchestrator<ForgotPasswordRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPatch("password")]
	[AllowAnonymous]
	public Task<IActionResult> PerformPasswordReset(
		ResetPasswordRequest data,
		[FromServices] IStatusActionOrchestrator<ResetPasswordRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPatch("change-password")]
	public Task<IActionResult> ChangePassword(
		ChangePasswordRequest data,
		[FromServices] IStatusActionOrchestrator<ChangePasswordRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPost("change-email")]
	public Task<IActionResult> ChangeEmail(
		InitiateEmailChangeRequest data,
		[FromServices] IStatusActionOrchestrator<InitiateEmailChangeRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPatch("email")]
	public Task<IActionResult> UpdateEmail(
		PerformEmailChangeRequest data,
		[FromServices] IStatusActionOrchestrator<PerformEmailChangeRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpGet("personal-data")]
	public async Task<IActionResult> GetPersonalData(
		[FromServices] IResultActor<PersonalDataResult> actor)
	{
		var result = await actor.Execute();

		if (result.Status != OperationStatus.Success
			|| result.Result?.PersonalDataFile?.Contents is null)
		{
			return new ObjectResult("Unable to download personal data")
			{
				StatusCode = StatusCodes.Status500InternalServerError
			};
		}

		var file = result.Result.PersonalDataFile;
		Response.Headers.Append("Content-Disposition", $"attachment; filename={file.Name}");

		return new FileContentResult(
			result.Result.PersonalDataFile.Contents,
			result.Result.PersonalDataFile.Mime);
	}

	[HttpGet("lockout-reasons")]
	[AllowAnonymous]
	public Task<IActionResult> GetLockoutReaons(
		[FromQuery] AccountLockoutRequest data,
		[FromServices] IGeneralActor<AccountLockoutRequest, AccountLockoutResult> service)
		=> Execute(() => service.Execute(data));
}
