#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientAddUsertoRoleProcessor : IStatusProcessor<AddUserToRoleRequest>
{
	private readonly IRestClient _client;
	private readonly IOperationResultNotifier _notifier;

	public ClientAddUsertoRoleProcessor(
		IRestClient client,
		IOperationResultNotifier notifier)
	{
		_client = client;
		_notifier = notifier;
	}

	public async Task<OperationResult<bool>> Process(AddUserToRoleRequest request)
		=> _notifier.HandleWebResult(
			await _client.Post<bool>("users/roles", request));
}
