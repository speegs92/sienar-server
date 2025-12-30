namespace Sienar.Identity.Hooks;

/// <summary>
/// Ensures that a username is unique
/// </summary>
public abstract class UsernameUniqueValidator<T>
	where T : class, ISienarIdentityUser<T>
{
	private readonly ISienarDbContext<T> _context;
	private readonly INotifier _notifier;

	/// <summary>
	/// Creates a new instance of <c>UsernameUniqueValidator</c>
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="notifier">The notifier</param>
	protected UsernameUniqueValidator(
		ISienarDbContext<T> context,
		INotifier notifier)
	{
		_context = context;
		_notifier = notifier;
	}

	/// <summary>
	/// Verifies whether the given username is unique
	/// </summary>
	/// <param name="username">The username to validate</param>
	/// <param name="id">The ID of the user, if applicable</param>
	/// <returns><see cref="OperationStatus.Success"/> if the username is unique, or <see cref="OperationStatus.Conflict"/> if it is not</returns>
	protected async Task<OperationStatus> VerifyUsernameUnique(string username, int id = 0)
	{
		username = username.ToNormalized();
		var usernameTaken = await _context.Users
			.AnyAsync(u => u.Id != id &&
				u.NormalizedUsername == username);

		if (usernameTaken)
		{
			_notifier.Error(CoreErrors.Account.UsernameTaken);
			return OperationStatus.Conflict;
		}

		return OperationStatus.Success;
	}
}
