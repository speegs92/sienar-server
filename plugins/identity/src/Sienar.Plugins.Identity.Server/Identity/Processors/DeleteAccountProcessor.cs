#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class DeleteAccountProcessor : IStatusProcessor<DeleteAccountRequest>
{
	private readonly IUserAccessor _userAccessor;
	private readonly ISienarDbContext _context;
	private readonly IPasswordManager _passwordManager;
	private readonly ISignInManager _signInManager;

	public DeleteAccountProcessor(
		IUserAccessor userAccessor,
		ISienarDbContext context,
		IPasswordManager passwordManager,
		ISignInManager signInManager)
	{
		_userAccessor = userAccessor;
		_context = context;
		_passwordManager = passwordManager;
		_signInManager = signInManager;
	}

	public async Task<OperationResult<bool>> Process(DeleteAccountRequest request)
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

		if (!await _passwordManager.VerifyPassword(user, request.Password))
		{
			return new(
				OperationStatus.Unauthorized,
				message: CoreErrors.Account.LoginFailedInvalid);
		}

		_context.Users.Remove(user);
		await _context.SaveChangesAsync();
		await _signInManager.SignOut();

		return new(
			OperationStatus.Success,
			true,
			"Account deleted successfully");
	}
}