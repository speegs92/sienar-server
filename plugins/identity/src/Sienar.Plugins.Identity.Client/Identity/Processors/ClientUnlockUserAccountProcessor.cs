#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientUnlockUserAccountProcessor : IStatusProcessor<UnlockUserAccountRequest>
{
	private readonly IRestClient _client;
	private readonly IOperationResultNotifier _notifier;

	public ClientUnlockUserAccountProcessor(
		IRestClient client,
		IOperationResultNotifier notifier)
	{
		_client = client;
		_notifier = notifier;
	}

	public async Task<OperationResult<bool>> Process(UnlockUserAccountRequest request)
		=> _notifier.HandleWebResult(
			await _client.Delete<bool>("users/lock", request));
}
