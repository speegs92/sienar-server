#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientLogoutProcessor : IStatusProcessor<LogoutRequest>
{
	private readonly IRestClient _client;
	private readonly IOperationResultNotifier _notifier;
	private readonly SienarAuthenticationStateProvider _authStateProvider;

	public ClientLogoutProcessor(
		IRestClient client,
		IOperationResultNotifier notifier,
		SienarAuthenticationStateProvider authStateProvider)
	{
		_client = client;
		_notifier = notifier;
		_authStateProvider = authStateProvider;
	}

	public async Task<OperationResult<bool>> Process(LogoutRequest request)
	{
		var loggedOutResult = await _client.Delete<bool>("account/login", request);

		if (loggedOutResult.Status is OperationStatus.Success)
		{
			_authStateProvider.Logout();
			await _client.RefreshCsrfToken();
		}

		return _notifier.HandleWebResult(loggedOutResult);
	}
}
