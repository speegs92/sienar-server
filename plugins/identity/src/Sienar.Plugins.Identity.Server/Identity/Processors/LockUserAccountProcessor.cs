#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class LockUserAccountProcessor<T> : IStatusProcessor<LockUserAccountRequest>
	where T : class, ISienarIdentityUser<T>
{
	private readonly ISienarDbContext<T> _context;
	private readonly IAccountEmailManager<T> _emailManager;

	public LockUserAccountProcessor(
		ISienarDbContext<T> context,
		IAccountEmailManager<T> emailManager)
	{
		_context = context;
		_emailManager = emailManager;
	}

	public async Task<OperationResult<bool>> Process(LockUserAccountRequest request)
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

		var reasons = await _context.LockoutReasons
			.Where(l => request.Reasons.Contains(l.Id))
			.ToListAsync();
		if (reasons.Count != request.Reasons.Count)
		{
			return new(
				OperationStatus.NotFound,
				message: CoreErrors.LockoutReason.NotFound);
		}

		user.LockoutReasons.AddRange(reasons);
		user.LockoutEnd = request.EndDate ?? DateTime.MaxValue;

		_context.Users.Update(user);
		await _context.SaveChangesAsync();

		if (!await _emailManager.SendAccountLockedEmail(user))
		{
			return new(
				OperationStatus.Success,
				true,
				$"User {user.Username} was locked successfully, but the email notification failed to send.");
		}

		return new(
			OperationStatus.Success,
			true,
			$"Locked user {user.Username} successfully");
	}
}