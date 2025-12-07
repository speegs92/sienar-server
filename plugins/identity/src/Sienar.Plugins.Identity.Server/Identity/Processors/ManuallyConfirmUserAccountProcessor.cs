#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ManuallyConfirmUserAccountProcessor
	: IStatusProcessor<ManuallyConfirmUserAccountRequest>
{
	private readonly ISienarDbContext _context;

	public ManuallyConfirmUserAccountProcessor(
		ISienarDbContext context)
	{
		_context = context;
	}

	public async Task<OperationResult<bool>> Process(ManuallyConfirmUserAccountRequest request)
	{
		var user = await _context.Users.FindAsync(request.UserId);
		if (user is null)
		{
			return new(
				OperationStatus.NotFound,
				message: CoreErrors.Account.NotFound);
		}

		if (user.EmailConfirmed)
		{
			return new(
				OperationStatus.Unprocessable,
				message: $"{user.Username}'s account is already confirmed");
		}

		user.EmailConfirmed = true;

		_context.Users.Update(user);
		await _context.SaveChangesAsync();

		return new(
			OperationStatus.Success,
			true,
			$"Confirmed {user.Username}'s account");
	}
}