#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Hooks;

/// <exclude />
public class EnsureAccountInfoUniqueValidator<T>
	: IStateValidator<T>, IStateValidator<RegisterRequest>
	where T : class, ISienarIdentityUser<T>
{
	private readonly ISienarDbContext<T> _context;
	private readonly INotifier _notifier;

	public EnsureAccountInfoUniqueValidator(
		ISienarDbContext<T> context,
		INotifier notifier)
	{
		_context = context;
		_notifier = notifier;
	}

	Task<OperationStatus> IStateValidator<T>.Validate(T request, ActionType type)
		=> UserIsUnique(
			request.Username,
			request.Email,
			request.PendingEmail,
			request.Id);

	Task<OperationStatus> IStateValidator<RegisterRequest>.Validate(
		RegisterRequest request,
		ActionType action)
		=> UserIsUnique(
			request.Username,
			request.Email);

	private async Task<OperationStatus> UserIsUnique(
		string username,
		string email,
		string? pendingEmail = null,
		int id = 0)
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

		email = email.ToNormalized();
		var emailTaken = await _context.Users
			.AnyAsync(u => u.Id != id &&
				(u.NormalizedEmail == email || u.NormalizedPendingEmail == email));
		if (emailTaken)
		{
			_notifier.Error(CoreErrors.Account.EmailTaken);
			return OperationStatus.Conflict;
		}

		return OperationStatus.Success;
	}
}