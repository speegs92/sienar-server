namespace Sienar.Identity.Hooks;

/// <summary>
/// Ensures that a user's email address is unique when a new user is created
/// </summary>
/// <typeparam name="T">The type of the user entity</typeparam>
public class EnsureEmailUniqueOnUpsert<T> :
	EmailUniqueValidator<T>,
	IStateValidator<T>
	where T : class, ISienarIdentityUser<T>
{
	/// <summary>
	/// Creates a new instance of <c>EnsureEmailUniqueOnUpsert</c>
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="notifier">The notifier</param>
	public EnsureEmailUniqueOnUpsert(
		ISienarDbContext<T> context,
		INotifier notifier)
		: base(context, notifier) {}

	/// <inheritdoc />
	public Task<OperationStatus> Validate(
		T user,
		ActionType action)
		=> VerifyEmailUnique(
			user.Email,
			user.Id);
}
