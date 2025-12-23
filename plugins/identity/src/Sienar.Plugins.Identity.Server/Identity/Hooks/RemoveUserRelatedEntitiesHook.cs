#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Hooks;

/// <exclude />
public class RemoveUserRelatedEntitiesHook<T> :
	IBeforeDeleteAction<T>,
	IBeforeStatusAction<DeleteAccountRequest>
	where T : class, ISienarIdentityUser<T>
{
	private readonly ISienarDbContext<T> _context;
	private readonly IUserAccessor _userAccessor;

	public RemoveUserRelatedEntitiesHook(
		ISienarDbContext<T> context,
		IUserAccessor userAccessor)
	{
		_context = context;
		_userAccessor = userAccessor;
	}

	public Task Handle(T user)
		=> HandleCore(user);

	public async Task Handle(DeleteAccountRequest request)
	{
		var userId = (await _userAccessor.GetUserId())!;
		var user = (await _context.Users.FindAsync(userId.Value))!;
		await HandleCore(user);
	}

	private async Task HandleCore(T user)
	{
		await _context
			.Entry(user)
			.Collection(u => u.VerificationCodes)
			.LoadAsync();

		user.VerificationCodes.Clear();

		_context.Update(user);
		await _context.SaveChangesAsync();
	}
}