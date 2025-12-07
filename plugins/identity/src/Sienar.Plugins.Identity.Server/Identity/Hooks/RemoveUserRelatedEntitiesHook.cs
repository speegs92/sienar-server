#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Security;

namespace Sienar.Identity.Hooks;

/// <exclude />
public class RemoveUserRelatedEntitiesHook :
	IBeforeDeleteAction<SienarUser>,
	IBeforeStatusAction<DeleteAccountRequest>
{
	private readonly ISienarDbContext _context;
	private readonly IUserAccessor _userAccessor;

	public RemoveUserRelatedEntitiesHook(
		ISienarDbContext context,
		IUserAccessor userAccessor)
	{
		_context = context;
		_userAccessor = userAccessor;
	}

	public Task Handle(SienarUser user)
		=> HandleCore(user);

	public async Task Handle(DeleteAccountRequest request)
	{
		var userId = (await _userAccessor.GetUserId())!;
		var user = (await _context.Users.FindAsync(userId.Value))!;
		await HandleCore(user);
	}

	private async Task HandleCore(SienarUser user)
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