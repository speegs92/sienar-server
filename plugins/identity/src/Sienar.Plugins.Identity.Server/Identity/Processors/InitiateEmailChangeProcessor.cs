#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class InitiateEmailChangeProcessor<T> : IStatusProcessor<InitiateEmailChangeRequest>
	where T : class, ISienarIdentityUser<T>
{
	private readonly ISienarDbContext<T> _context;
	private readonly IPasswordManager<T> _passwordManager;
	private readonly IAccountEmailManager<T> _emailManager;
	private readonly IUserAccessor _userAccessor;
	private readonly SienarOptions _sienarOptions;
	private readonly LoginOptions _loginOptions;

	public InitiateEmailChangeProcessor(
		ISienarDbContext<T> context,
		IPasswordManager<T> passwordManager,
		IAccountEmailManager<T> emailManager,
		IUserAccessor userAccessor,
		IOptions<SienarOptions> sienarOptions,
		IOptions<LoginOptions> loginOptions)
	{
		_context = context;
		_passwordManager = passwordManager;
		_emailManager = emailManager;
		_userAccessor = userAccessor;
		_sienarOptions = sienarOptions.Value;
		_loginOptions = loginOptions.Value;
	}

	public async Task<OperationResult<bool>> Process(InitiateEmailChangeRequest request)
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

		if (!await _passwordManager.VerifyPassword(user, request.ConfirmPassword))
		{
			return new(
				OperationStatus.Unauthorized,
				message: CoreErrors.Account.LoginFailedInvalid);
		}

		var shouldSendConfirmationEmail = _loginOptions.RequireConfirmedAccount &&
			_sienarOptions.EnableEmail;

		if (shouldSendConfirmationEmail)
		{
			user.PendingEmail = request.Email;
			user.NormalizedPendingEmail = request.Email.ToNormalized();
		}
		else
		{
			user.Email = request.Email;
		}

		_context.Users.Update(user);
		await _context.SaveChangesAsync();

		if (shouldSendConfirmationEmail)
		{
			if (!await _emailManager.SendEmailChangeConfirmationEmail(user))
			{
				return new(
					OperationStatus.Unknown,
					true,
					CoreErrors.Email.FailedToSend);
			}
		}

		return new(
			OperationStatus.Success,
			true,
			"Email change requested");
	}
}