#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class LoginProcessor<T> : IGeneralProcessor<LoginRequest, LoginResult>
	where T : class, ISienarIdentityUser<T>
{
	private readonly ISienarDbContext<T> _context;
	private readonly IPasswordManager<T> _passwordManager;
	private readonly ISignInManager<T> _signInManager;
	private readonly IAccountEmailManager<T> _emailManager;
	private readonly IVerificationCodeManager<T> _vcManager;
	private readonly LoginOptions _loginOptions;
	private readonly SienarOptions _appOptions;

	public LoginProcessor(
		ISienarDbContext<T> context,
		IPasswordManager<T> passwordManager,
		ISignInManager<T> signInManager,
		IAccountEmailManager<T> emailManager,
		IVerificationCodeManager<T> vcManager,
		IOptions<LoginOptions> loginOptions,
		IOptions<SienarOptions> appOptions)
	{
		_context = context;
		_passwordManager = passwordManager;
		_signInManager = signInManager;
		_emailManager = emailManager;
		_vcManager = vcManager;
		_loginOptions = loginOptions.Value;
		_appOptions = appOptions.Value;
	}

	public async Task<OperationResult<LoginResult>> Process(LoginRequest request)
	{
		var normalizedAccountName = request.AccountName.ToNormalized();
		var user = await _context.Users
			.FirstOrDefaultAsync(
				u => u.NormalizedUsername == normalizedAccountName ||
				u.NormalizedEmail == normalizedAccountName);
		if (user == null)
		{
			return new(
				OperationStatus.NotFound,
				message: CoreErrors.Account.LoginFailedNotFound);
		}

		if (user.IsLockedOut())
		{
			var code = await _vcManager.CreateCode(user, VerificationCodeTypes.ViewLockoutReasons);
			return new(
				OperationStatus.Unauthorized,
				new()
				{
					UserId = user.Id,
					VerificationCode = code.Code
				},
				message: CoreErrors.Account.AccountLocked);
		}

		if (!await _passwordManager.VerifyPassword(user, request.Password))
		{
			user.LoginFailedCount++;
			if (user.LoginFailedCount >= _loginOptions.MaxFailedLoginAttempts)
			{
				user.LoginFailedCount = 0;
				user.LockoutEnd = DateTime.UtcNow + _loginOptions.LockoutTimespan;
				var code = await _vcManager.CreateCode(user, VerificationCodeTypes.ViewLockoutReasons);

				_context.Users.Update(user);
				await _context.SaveChangesAsync();
				await _emailManager.SendAccountLockedEmail(user);
				return new(
					OperationStatus.Unauthorized,
					new()
					{
						UserId = user.Id,
						VerificationCode = code.Code
					},
					message: CoreErrors.Account.LoginFailedLocked);
			}

			_context.Users.Update(user);
			await _context.SaveChangesAsync();

			return new(
				OperationStatus.Unauthorized,
				message: CoreErrors.Account.LoginFailedInvalid);
		}

		// Still check if email is confirmed no matter what
		// That way, users registered when confirmation was
		// required still need to confirm
		if (!user.EmailConfirmed)
		{
			if (_appOptions.EnableEmail)
			{
				await _emailManager.SendWelcomeEmail(user);
				return new(
					OperationStatus.Unauthorized,
					message: CoreErrors.Account.LoginFailedNotConfirmed);
			}

			return new(
				OperationStatus.Unauthorized,
				message: CoreErrors.Account.LoginFailedNotConfirmedEmailDisabled);
		}

		// User is authenticated and able to log in
		user.LoginFailedCount = 0;
		user.LockoutEnd = null;
		_context.Users.Update(user);
		await _context.SaveChangesAsync();

		// Save the token to the token cache
		await _signInManager.SignIn(user, request.RememberMe);

		return new(message: "Logged in successfully");
	}
}