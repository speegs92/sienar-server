namespace Sienar.Identity.Hooks;

/// <summary>
/// Removes Sienar Identity entity relations when a user deletes their own account
/// </summary>
/// <typeparam name="T">The type of the user entity</typeparam>
public class RemoveIdentityRelationsOnOwnAccountDeleted<T> : 
	RemoveIdentityRelationsOnDeleted<T>,
	IBeforeStatusAction<DeleteAccountRequest>
	where T : class, ISienarIdentityUser<T>
{
	private readonly IUserAccessor _userAccessor;

	/// <summary>
	/// Creates a new instance of <c>RemoveIdentityRelationsOnOwnAccountDeleted</c>
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="userAccessor">The user accessor</param>
	public RemoveIdentityRelationsOnOwnAccountDeleted(
		ISienarDbContext<T> context,
		IUserAccessor userAccessor)
		: base(context)
		=> _userAccessor = userAccessor;

	/// <inheritdoc />
	public async Task Handle(DeleteAccountRequest request)
	{
		var userId = await _userAccessor.GetUserId();
		var user = await Context.Users.FindAsync(userId!.Value);
		await RemoveRelations(user!);
	}
}
