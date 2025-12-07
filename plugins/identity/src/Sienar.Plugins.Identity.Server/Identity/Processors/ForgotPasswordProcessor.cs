#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Data;
using Sienar.Email;
using Sienar.Errors;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ForgotPasswordProcessor : IStatusProcessor<ForgotPasswordRequest>
{
	private readonly ISienarDbContext _context;
	private readonly IAccountEmailManager _emailManager;
	private readonly SienarOptions _options;

	public ForgotPasswordProcessor(
		ISienarDbContext context,
		IAccountEmailManager emailManager,
		IOptions<SienarOptions> options)
	{
		_context = context;
		_emailManager = emailManager;
		_options = options.Value;
	}

	public async Task<OperationResult<bool>> Process(ForgotPasswordRequest request)
	{
		var normalizedAccountName = request.AccountName.ToNormalized();
		var user = await _context.Users.FirstOrDefaultAsync(
				u => u.NormalizedEmail == normalizedAccountName ||
				u.NormalizedUsername == normalizedAccountName);

		// If the user doesn't exist, they don't need to know
		// Just return success
		if (user is null)
		{
			return new(OperationStatus.Success, true);
		}

		if (user.IsLockedOut())
		{
			return new(
				OperationStatus.Unauthorized,
				message: CoreErrors.Account.AccountLocked);
		}

		// They don't need to know whether the user exists or not
		// so if the user isn't null, send the email
		// otherwise, just return Ok anyway
		if (_options.EnableEmail)
		{
			if (!await _emailManager.SendPasswordResetEmail(user))
			{
				return new(
					OperationStatus.Unknown,
					message: CoreErrors.Email.FailedToSend);
			}
		}

		return new(
			OperationStatus.Success,
			true,
			"Password reset requested");
	}
}