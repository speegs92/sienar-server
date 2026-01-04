#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class LogoutProcessor<T> : IStatusProcessor<LogoutRequest>
	where T : class, ISienarIdentityUser<T>
{
	private readonly ISignInManager<T> _signInManager;
	private readonly INotifier _notifier;

	public LogoutProcessor(
		ISignInManager<T> signInManager,
		INotifier notifier)
	{
		_signInManager = signInManager;
		_notifier = notifier;
	}

	public async Task<OperationResult<bool>> Process(LogoutRequest request)
	{
		await _signInManager.SignOut();
		return new(
			OperationStatus.Success,
			true,
			"Logged out successfully");
	}
}