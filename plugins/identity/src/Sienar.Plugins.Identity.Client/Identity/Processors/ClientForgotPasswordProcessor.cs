#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientForgotPasswordProcessor : IStatusProcessor<ForgotPasswordRequest>
{
	private readonly IRestClient _client;
	private readonly IOperationResultNotifier _notifier;

	public ClientForgotPasswordProcessor(
		IRestClient client,
		IOperationResultNotifier notifier)
	{
		_client = client;
		_notifier = notifier;
	}

	public async Task<OperationResult<bool>> Process(ForgotPasswordRequest request)
		=> _notifier.HandleWebResult(
			await _client.Delete<bool>("account/password", request));
}
