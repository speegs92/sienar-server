#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class GetLockoutReasonsProcessor<T>
	: IGeneralProcessor<AccountLockoutRequest, AccountLockoutResult>
	where T : class, ISienarIdentityUser<T>
{
	private readonly ISienarDbContext<T> _context;
	private readonly IVerificationCodeManager<T> _vcManager;
	private readonly IMapper<LockoutReason<T>, LockoutReasonDto> _lockoutReasonMapper;

	public GetLockoutReasonsProcessor(
		ISienarDbContext<T> context,
		IVerificationCodeManager<T> vcManager,
		IMapper<LockoutReason<T>, LockoutReasonDto> lockoutReasonMapper)
	{
		_context = context;
		_vcManager = vcManager;
		_lockoutReasonMapper = lockoutReasonMapper;
	}

	public async Task<OperationResult<AccountLockoutResult>> Process(
		AccountLockoutRequest request)
	{
		var user = await _context.Users
			.Include(u => u.VerificationCodes)
			.Include(u => u.LockoutReasons)
			.FirstOrDefaultAsync(u => u.Id == request.UserId);

		if (user is null)
		{
			return new(
				OperationStatus.NotFound,
				message: StatusMessages.Crud<T>.NotFound(request.UserId));
		}

		var status = await _vcManager.VerifyCode(
			user,
			VerificationCodeTypes.ViewLockoutReasons,
			request.VerificationCode,
			true);

		if (status is VerificationCodeStatus.Invalid)
		{
			return new(
				OperationStatus.NotFound,
				message: CoreErrors.Account.VerificationCodeInvalid);
		}

		if (status is VerificationCodeStatus.Expired)
		{
			return new(
				OperationStatus.Unprocessable,
				message: CoreErrors.Account.VerificationCodeExpired);
		}

		if (user.LockoutReasons.Count == 0)
		{
			user.LockoutReasons.Add(new()
			{
				Reason = "You have attempted to log in with invalid credentials too many times"
			});
		}

		var lockoutReasons = new List<LockoutReasonDto>();

		foreach (var reason in user.LockoutReasons)
		{
			var dto = new LockoutReasonDto();
			_lockoutReasonMapper.Map(reason, dto);
			lockoutReasons.Add(dto);
		}

		return new(
			OperationStatus.Success,
			new()
			{
				LockoutReasons = lockoutReasons,
				LockoutEnd = user.LockoutEnd == DateTime.MaxValue ? null : user.LockoutEnd
			});
	}
}