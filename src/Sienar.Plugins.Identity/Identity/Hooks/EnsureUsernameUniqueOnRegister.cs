namespace Sienar.Identity.Hooks;

/// <summary>
/// Ensures that an account's username is unique when a new user registers
/// </summary>
/// <typeparam name="T">The type of the user entity</typeparam>
public class EnsureUsernameUniqueOnRegister<T> :
	UsernameUniqueValidator<T>,
	IStateValidator<RegisterRequest>
	where T : class, ISienarIdentityUser<T>
{
	/// <summary>
	/// Creates a new instance of <c>EnsureUsernameUniqueOnRegister</c>
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="notifier">The notifier</param>
	public EnsureUsernameUniqueOnRegister(
		ISienarDbContext<T> context,
		INotifier notifier)
		: base(context, notifier) {}

	/// <inheritdoc />
	public Task<OperationStatus> Validate(
		RegisterRequest request,
		ActionType action)
		=> VerifyUsernameUnique(request.Username);
}
