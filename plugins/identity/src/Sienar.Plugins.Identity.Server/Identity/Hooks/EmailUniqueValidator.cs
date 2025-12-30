namespace Sienar.Identity.Hooks;

/// <summary>
/// Ensures that an email address is unique
/// </summary>
/// <typeparam name="T">The type of the user entity</typeparam>
public abstract class EmailUniqueValidator<T>
	where T : class, ISienarIdentityUser<T>
{
	private readonly ISienarDbContext<T> _context;
	private readonly INotifier _notifier;

	/// <summary>
	/// Creates a new instance of <c>EmailUniqueValidator</c>
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="notifier">The notifier</param>
	protected EmailUniqueValidator(
		ISienarDbContext<T> context,
		INotifier notifier)
	{
		_context = context;
		_notifier = notifier;
	}

	/// <summary>
	/// Verifies whether the given email is unique
	/// </summary>
	/// <param name="email">The email to validate</param>
	/// <param name="id">The ID of the user, if applicable</param>
	/// <returns><see cref="OperationStatus.Success"/> if the email is unique, or <see cref="OperationStatus.Conflict"/> if it is not</returns>
	protected async Task<OperationStatus> VerifyEmailUnique(
		string email,
		int id = 0)
	{
		email = email.ToNormalized();
		var emailTaken = await _context.Users
			.AnyAsync(
				u => u.Id != id &&
				(u.NormalizedEmail == email || u.NormalizedPendingEmail == email));

		if (emailTaken)
		{
			_notifier.Error(CoreErrors.Account.EmailTaken);
			return OperationStatus.Conflict;
		}

		return OperationStatus.Success;
	}
}
