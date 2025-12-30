namespace Sienar.Identity.Hooks;

/// <summary>
/// Removes Sienar Identity entity relations when a user's account is deleted by someone else
/// </summary>
/// <typeparam name="T">The type of the user entity</typeparam>
public class RemoveIdentityRelationsOnUserDeleted<T> :
	RemoveIdentityRelationsOnDeleted<T>,
	IBeforeDeleteAction<T>
	where T : class, ISienarIdentityUser<T>
{
	/// <summary>
	/// Creates a new instance of <c>RemoveIdentityRelationsOnUserDeleted</c>
	/// </summary>
	/// <param name="context">The database context</param>
	public RemoveIdentityRelationsOnUserDeleted(
		ISienarDbContext<T> context)
		: base(context) {}

	/// <inheritdoc />
	public Task Handle(T user)
		=> RemoveRelations(user);
}