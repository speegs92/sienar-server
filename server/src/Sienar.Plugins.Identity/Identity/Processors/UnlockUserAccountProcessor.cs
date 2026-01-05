#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class UnlockUserAccountProcessor<T> : IStatusProcessor<UnlockUserAccountRequest>
	where T : class, ISienarIdentityUser<T>
{
	private readonly ISienarDbContext<T> _context;

	public UnlockUserAccountProcessor(ISienarDbContext<T> context)
	{
		_context = context;
	}

	public async Task<OperationResult<bool>> Process(UnlockUserAccountRequest request)
	{
		var user = await _context.Users
			.Include(u => u.LockoutReasons)
			.FirstOrDefaultAsync(u => u.Id == request.UserId);
		if (user is null)
		{
			return new(
				OperationStatus.NotFound,
				message: CoreErrors.Account.NotFound);
		}

		user.LockoutEnd = null;
		user.LockoutReasons.Clear();

		_context.Users.Update(user);
		await _context.SaveChangesAsync();

		return new(
			OperationStatus.Success,
			true,
			$"User {user.Username}'s account was unlocked successfully");
	}
}