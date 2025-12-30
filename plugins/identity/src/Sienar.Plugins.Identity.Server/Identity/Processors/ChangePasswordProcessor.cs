#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class ChangePasswordProcessor<T> : IStatusProcessor<ChangePasswordRequest>
	where T : class, ISienarIdentityUser<T>
{
	private readonly ISienarDbContext<T> _context;
	private readonly IUserAccessor _userAccessor;
	private readonly IPasswordManager<T> _passwordManager;

	public ChangePasswordProcessor(
		ISienarDbContext<T> context,
		IUserAccessor userAccessor,
		IPasswordManager<T> passwordManager)
	{
		_context = context;
		_userAccessor = userAccessor;
		_passwordManager = passwordManager;
	}

	public async Task<OperationResult<bool>> Process(ChangePasswordRequest request)
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

		if (!await _passwordManager.VerifyPassword(user, request.CurrentPassword))
		{
			return new(
				OperationStatus.Unauthorized,
				message: CoreErrors.Account.LoginFailedInvalid);
		}

		await _passwordManager.UpdatePassword(user, request.NewPassword);

		return new(
			OperationStatus.Success,
			true,
			"Password changed successfully");
	}
}