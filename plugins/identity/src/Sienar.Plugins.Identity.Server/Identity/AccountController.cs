#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1822 // Member can be marked as static

using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Sienar.Identity;

/// <exclude />
[ApiController]
[Route("/api/account")]
[Authorize]
public class AccountController : ControllerBase
{

	[HttpPost]
	[AllowAnonymous]
	[UsedImplicitly]
	public Task<IActionResult> Register(
		RegisterRequest data,
		[FromServices] IStatusActionOrchestrator<RegisterRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpGet]
	[UsedImplicitly]
	public Task<IActionResult> GetAccountData(
		[FromServices] IResultActionOrchestrator<AccountDataResult> orchestrator)
		=> orchestrator.Execute();

	[HttpDelete]
	[UsedImplicitly]
	public Task<IActionResult> DeleteAccount(
		DeleteAccountRequest data,
		[FromServices] IStatusActionOrchestrator<DeleteAccountRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPost("confirm")]
	[AllowAnonymous]
	[UsedImplicitly]
	public Task<IActionResult> Confirm(
		ConfirmAccountRequest data,
		[FromServices] IStatusActionOrchestrator<ConfirmAccountRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPost("login")]
	[AllowAnonymous]
	[UsedImplicitly]
	public Task<IActionResult> Login(
		LoginRequest data,
		[FromServices] IGeneralActionOrchestrator<LoginRequest, LoginResult> orchestrator)
		=> orchestrator.Execute(data);

	[HttpDelete("login")]
	[UsedImplicitly]
	public Task<IActionResult> Logout(
		LogoutRequest data,
		[FromServices] IStatusActionOrchestrator<LogoutRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpDelete("password")]
	[AllowAnonymous]
	[UsedImplicitly]
	public Task<IActionResult> RequestPasswordReset(
		ForgotPasswordRequest data,
		[FromServices] IStatusActionOrchestrator<ForgotPasswordRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPatch("password")]
	[AllowAnonymous]
	[UsedImplicitly]
	public Task<IActionResult> PerformPasswordReset(
		ResetPasswordRequest data,
		[FromServices] IStatusActionOrchestrator<ResetPasswordRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPatch("change-password")]
	[UsedImplicitly]
	public Task<IActionResult> ChangePassword(
		ChangePasswordRequest data,
		[FromServices] IStatusActionOrchestrator<ChangePasswordRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPost("change-email")]
	[UsedImplicitly]
	public Task<IActionResult> ChangeEmail(
		InitiateEmailChangeRequest data,
		[FromServices] IStatusActionOrchestrator<InitiateEmailChangeRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpPatch("email")]
	[UsedImplicitly]
	public Task<IActionResult> UpdateEmail(
		PerformEmailChangeRequest data,
		[FromServices] IStatusActionOrchestrator<PerformEmailChangeRequest> orchestrator)
		=> orchestrator.Execute(data);

	[HttpGet("personal-data")]
	[UsedImplicitly]
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
	[UsedImplicitly]
	public Task<IActionResult> GetLockoutReaons(
		[FromQuery] AccountLockoutRequest data,
		[FromServices] IGeneralActionOrchestrator<AccountLockoutRequest, AccountLockoutResult> orchestrator)
		=> orchestrator.Execute(data);
}
