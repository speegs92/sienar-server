namespace Sienar.Identity.Processors;

public class ClientAccountLockoutProcessor : IProcessor<AccountLockoutRequest, AccountLockoutResult>
{
	private readonly IRestClient _client;
	private readonly IOperationResultNotifier _notifier;

	public ClientAccountLockoutProcessor(
		IRestClient client,
		IOperationResultNotifier notifier)
	{
		_client = client;
		_notifier = notifier;
	}

	public async Task<OperationResult<AccountLockoutResult?>> Process(AccountLockoutRequest request)
		=> _notifier.HandleWebResult(
			await _client.Get<AccountLockoutResult>("account/lockout-reasons", request));
}
