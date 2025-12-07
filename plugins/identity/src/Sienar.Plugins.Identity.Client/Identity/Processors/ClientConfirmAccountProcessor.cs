#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientConfirmAccountProcessor : IStatusProcessor<ConfirmAccountRequest>
{
	private readonly IRestClient _client;
	private readonly IOperationResultNotifier _notifier;

	public ClientConfirmAccountProcessor(
		IRestClient client,
		IOperationResultNotifier notifier)
	{
		_client = client;
		_notifier = notifier;
	}

	public async Task<OperationResult<bool>> Process(ConfirmAccountRequest request)
		=> _notifier.HandleWebResult(
			await _client.Post<bool>("account/confirm", request));
}
