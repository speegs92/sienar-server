#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientLoginProcessor : IProcessor<LoginRequest, LoginResult>
{
	private readonly IRestClient _client;
	private readonly IOperationResultNotifier _notifier;
	private readonly IResultProcessor<AccountDataResult> _loadUserDataProcessor;

	public ClientLoginProcessor(
		IRestClient client,
		IOperationResultNotifier notifier,
		IResultProcessor<AccountDataResult> loadUserDataProcessor)
	{
		_client = client;
		_notifier = notifier;
		_loadUserDataProcessor = loadUserDataProcessor;
	}

	public async Task<OperationResult<LoginResult>> Process(LoginRequest request)
	{
		var result = await _client.Post<LoginResult>("account/login", request);

		if (result.Status == OperationStatus.Success)
		{
			await _loadUserDataProcessor.Process();
			await _client.RefreshCsrfToken();
		}

		return _notifier.HandleWebResult(result);
	}
}
