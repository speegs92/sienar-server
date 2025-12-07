#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientInitiateEmailChangeProcessor : IStatusProcessor<InitiateEmailChangeRequest>
{
	private readonly IRestClient _client;
	private readonly IOperationResultNotifier _notifier;

	public ClientInitiateEmailChangeProcessor(
		IRestClient client,
		IOperationResultNotifier notifier)
	{
		_client = client;
		_notifier = notifier;
	}

	public async Task<OperationResult<bool>> Process(InitiateEmailChangeRequest request)
		=> _notifier.HandleWebResult(
			await _client.Post<bool>("account/change-email", request));
}