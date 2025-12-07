#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class ResetPasswordProcessor : IStatusProcessor<ResetPasswordRequest>
{
	private readonly ISienarDbContext _context;
	private readonly IPasswordManager _passwordManager;
	private readonly IVerificationCodeManager _vcManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly SienarOptions _options;

	public ResetPasswordProcessor(
		ISienarDbContext context,
		IPasswordManager passwordManager,
		IVerificationCodeManager vcManager,
		IAccountEmailManager emailManager,
		IOptions<SienarOptions> options)
	{
		_context = context;
		_passwordManager = passwordManager;
		_vcManager = vcManager;
		_emailManager = emailManager;
		_options = options.Value;
	}

	public async Task<OperationResult<bool>> Process(ResetPasswordRequest request)
	{
		var user = await _context.Users.FindAsync(request.UserId);
		if (user == null)
		{
			return new(
				OperationStatus.NotFound,
				message: CoreErrors.Account.AccountErrorInvalidId);
		}

		var status = await _vcManager.VerifyCode(
			user,
			VerificationCodeTypes.PasswordReset,
			request.VerificationCode,
			true);

		if (status == VerificationCodeStatus.Invalid)
		{
			return new(
				OperationStatus.NotFound,
				message: CoreErrors.Account.VerificationCodeInvalid);
		}

		if (status == VerificationCodeStatus.Expired)
		{
			if (_options.EnableEmail)
			{
				await _emailManager.SendPasswordResetEmail(user);
				return new(
					OperationStatus.Unprocessable,
					message: CoreErrors.Account.VerificationCodeExpired);
			}

			return new(
				OperationStatus.Unprocessable,
                message: CoreErrors.Account.VerificationCodeExpiredEmailDisabled);
		}

		// Code was valid
		await _passwordManager.UpdatePassword(user, request.NewPassword);

		return new(
			OperationStatus.Success,
			true,
			"Password reset successfully");
	}
}