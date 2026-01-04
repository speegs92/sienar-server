namespace Sienar.Identity.Hooks;

/// <summary>
/// Ensures that a user's username is unique when a new user is created
/// </summary>
/// <typeparam name="T">The type of the user entity</typeparam>
public class EnsureUsernameUniqueOnUpsert<T> :
	UsernameUniqueValidator<T>,
	IStateValidator<T>
	where T : class, ISienarIdentityUser<T>
{
	/// <summary>
	/// Creates a new instance of <c>EnsureUsernameUniqueOnUpsert</c>
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="notifier">The notifier</param>
	public EnsureUsernameUniqueOnUpsert(
		ISienarDbContext<T> context,
		INotifier notifier)
		: base(context, notifier) {}

	/// <inheritdoc />
	public Task<OperationStatus> Validate(
		T user,
		ActionType action)
		=> VerifyUsernameUnique(
			user.Username,
			user.Id);
}
