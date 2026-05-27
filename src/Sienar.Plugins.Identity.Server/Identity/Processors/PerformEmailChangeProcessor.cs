#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class PerformEmailChangeProcessor<T>
	: IStatusProcessor<PerformEmailChangeRequest>
	where T : class, ISienarIdentityUser<T>
{
	private readonly ISienarDbContext<T> _context;
	private readonly IUserAccessor _userAccessor;
	private readonly IVerificationCodeManager<T> _vcManager;
	private readonly IAccountEmailManager<T> _emailManager;
	private readonly SienarOptions _sienarOptions;

	public PerformEmailChangeProcessor(
		ISienarDbContext<T> context,
		IUserAccessor userAccessor,
		IVerificationCodeManager<T> vcManager,
		IAccountEmailManager<T> emailManager,
		IOptions<SienarOptions> sienarOptions)
	{
		_context = context;
		_userAccessor = userAccessor;
		_vcManager = vcManager;
		_emailManager = emailManager;
		_sienarOptions = sienarOptions.Value;
	}

	public async Task<OperationResult<bool>> Process(PerformEmailChangeRequest request)
	{
		var userId = await _userAccessor.GetUserId();
		if (!userId.HasValue)
		{
			return new(
				OperationStatus.Unauthorized,
				message: CoreErrors.Account.LoginRequired);
		}

		var user = await _context.Users.FindAsync(userId.Value);
		if (user is null)
		{
			return new(
				OperationStatus.Unauthorized,
				message: CoreErrors.Account.LoginRequired);
		}

		if (user.Id != request.UserId)
		{
			return new(
				OperationStatus.Unprocessable,
				message: CoreErrors.Account.AccountErrorWrongId);
		}

		if (string.IsNullOrEmpty(user.PendingEmail))
		{
			return new(
				OperationStatus.Unprocessable,
				message: CoreErrors.Account.NoPendingEmail);
		}

		var status = await _vcManager.VerifyCode(
			user,
			VerificationCodeTypes.ChangeEmail,
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
			if (_sienarOptions.EnableEmail)
			{
				await _emailManager.SendEmailChangeConfirmationEmail(user);
				return new(
					OperationStatus.Unprocessable,
					message: CoreErrors.Account.VerificationCodeExpired);
			}

			return new(
				OperationStatus.Unprocessable,
				message: CoreErrors.Account.VerificationCodeExpiredEmailDisabled);
		}

		// Code was valid
		user.Email = user.PendingEmail;
		user.NormalizedEmail = user.NormalizedPendingEmail!.ToNormalized();
		user.PendingEmail = null;
		user.NormalizedPendingEmail = null;

		_context.Users.Update(user);
		await _context.SaveChangesAsync();

		return new(
			OperationStatus.Success,
			true,
			"Email changed successfully");
	}
}