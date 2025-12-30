namespace Sienar.Identity.Hooks;

/// <summary>
/// Removes a user's Sienar Identity related entities when a user is deleted
/// </summary>
public abstract class RemoveIdentityRelationsOnDeleted<T>
	where T : class, ISienarIdentityUser<T>
{
	/// <summary>
	/// The Sienar Identity database context
	/// </summary>
	protected readonly ISienarDbContext<T> Context;

	/// <summary>
	/// Creates a new instance of <c>RemoveIdentityRelationsOnDeleted&lt;T&gt;</c>
	/// </summary>
	/// <param name="context">The database context</param>
	protected RemoveIdentityRelationsOnDeleted(
		ISienarDbContext<T> context)
		=> Context = context;

	/// <summary>
	/// Removes Sienar Identity relations from the given user entry
	/// </summary>
	/// <param name="user">The user for whom to remove Identity relations</param>
	protected async Task RemoveRelations(T user)
	{
		await Context
			.Entry(user)
			.Collection(u => u.VerificationCodes)
			.LoadAsync();

		user.VerificationCodes.Clear();

		Context.Update(user);
		await Context.SaveChangesAsync();
	}
}
