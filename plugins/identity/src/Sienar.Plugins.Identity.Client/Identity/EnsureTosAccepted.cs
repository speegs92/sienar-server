namespace Sienar.Identity;

/// <summary>
/// Ensures that a new user has accepted the Terms of Service prior to submitting their registration request
/// </summary>
public class EnsureTosAccepted : IStateValidator<RegisterRequest>
{
	private readonly INotifier _notifier;

	/// <summary>
	/// Creates a new instance of <c>EnsureTosAccepted</c>
	/// </summary>
	/// <param name="notifier"></param>
	public EnsureTosAccepted(INotifier notifier)
		=> _notifier = notifier;

	public Task<OperationStatus> Validate(
		RegisterRequest request,
		ActionType action)
	{
		var status = OperationStatus.Success;

		if (!request.AcceptTos)
		{
			status = OperationStatus.Unprocessable;
			_notifier.Error(CoreErrors.Account.MustAcceptTos);
		}

		return Task.FromResult(status);
	}
}
