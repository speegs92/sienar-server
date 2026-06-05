#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Microsoft.AspNetCore.Identity;

namespace Sienar.Identity.Processors;

/// <exclude />
public class RegisterProcessor<T> : IStatusProcessor<RegisterRequest>
	where T : class, ISienarIdentityUser<T>, new()
{
	private readonly ISienarDbContext<T> _context;
	private readonly IAccountEmailManager<T> _emailManager;
	private readonly IPasswordHasher<T> _passwordHasher;
	private readonly LoginOptions _loginOptions;
	private readonly SienarOptions _appOptions;

	public RegisterProcessor(
		ISienarDbContext<T> context,
		IAccountEmailManager<T> emailManager,
		IPasswordHasher<T> passwordHasher,
		IOptions<LoginOptions> loginOptions,
		IOptions<SienarOptions> appOptions)
	{
		_context = context;
		_emailManager = emailManager;
		_passwordHasher = passwordHasher;
		_loginOptions = loginOptions.Value;
		_appOptions = appOptions.Value;
	}

	public async Task<OperationResult<bool>> Process(RegisterRequest request)
	{
		// Checks passed. Make a new user
		var user = new T
		{
			Username = request.Username,
			NormalizedUsername = request.Username.ToNormalized(),
			Email = request.Email,
			NormalizedEmail = request.Email.ToNormalized(),
			ConcurrencyStamp = Guid.NewGuid()
		};

		user.PasswordHash = _passwordHasher.HashPassword(
			user,
			request.Password);

		var shouldSendRegistrationEmail = 
			_loginOptions.RequireConfirmedAccount &&
			_appOptions.EnableEmail;
		if (!shouldSendRegistrationEmail)
		{
			user.EmailConfirmed = true;
		}

		// Try to create that user with the given password
		await _context.Users.AddAsync(user);
		await _context.SaveChangesAsync();

		if (shouldSendRegistrationEmail)
		{
			await _emailManager.SendWelcomeEmail(user);
		}

		return new(
			OperationStatus.Success,
			true,
			"Registered successfully");
	}
}