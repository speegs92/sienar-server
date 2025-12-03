#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientRemoveUserFromRoleProcessor : IStatusProcessor<RemoveUserFromRoleRequest>
{
	private readonly IRestClient _client;
	private readonly IOperationResultNotifier _notifier;

	public ClientRemoveUserFromRoleProcessor(
		IRestClient client,
		IOperationResultNotifier notifier)
	{
		_client = client;
		_notifier = notifier;
	}

	public async Task<OperationResult<bool>> Process(RemoveUserFromRoleRequest request)
		=> _notifier.HandleWebResult(
			await _client.Delete<bool>("users/roles", request));
}
