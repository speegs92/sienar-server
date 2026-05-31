namespace Sienar.Identity.Hooks;

/// <summary>
/// ensures that an account's email is unique when a new user registers
/// </summary>
/// <typeparam name="T">The type of the user entity</typeparam>
public class EnsureEmailUniqueOnRegister<T> :
	EmailUniqueValidator<T>,
	IStateValidator<RegisterRequest>
	where T : class, ISienarIdentityUser<T>
{
	/// <summary>
	/// Creates a new instance of <c>EnsureEmailUniqueOnRegister</c>
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="notifier">The notifier</param>
	public EnsureEmailUniqueOnRegister(
		ISienarDbContext<T> context,
		INotifier notifier)
		: base(context, notifier) {}

	/// <inheritdoc />
	public Task<OperationStatus> Validate(
		RegisterRequest request,
		ActionType action)
		=> VerifyEmailUnique(request.Email);
}
