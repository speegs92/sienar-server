#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientLockUserAccountProcessor : IStatusProcessor<LockUserAccountRequest>
{
	private readonly IRestClient _client;
	private readonly IOperationResultNotifier _notifier;

	public ClientLockUserAccountProcessor(
		IRestClient client,
		IOperationResultNotifier notifier)
	{
		_client = client;
		_notifier = notifier;
	}

	public async Task<OperationResult<bool>> Process(LockUserAccountRequest request)
		=> _notifier.HandleWebResult(
			await _client.Patch<bool>("users/lock", request));
}
