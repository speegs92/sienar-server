using System;
using System.Reflection;
using System.Threading.Tasks;
using Sienar.Infrastructure;

namespace Sienar.Data;

/// <inheritdoc />
/// <remarks>
/// The <c>DefaultClientStatusProcessor</c> automatically handles DTOs which have been properly decorated with <see cref="RestEndpointAttribute"/>. In order to use this processor on a DTO without any special registration, the <c>TRequest</c> <b>must</b> be decorated with a <see cref="RestEndpointAttribute"/>, and it <b>must</b> supply both arguments. Without both arguments, this processor won't know how to communicate the DTO to the API. 
/// </remarks>
public class DefaultClientStatusProcessor<T>
	: IStatusProcessor<T>
	where T : IRequest
{
	private readonly IRestClient _client;
	private readonly IOperationResultNotifier _notifier;

	/// <summary>
	/// Creates a new instance of <c>DefaultClientStatusProcessor</c>
	/// </summary>
	/// <param name="client">The REST client</param>
	/// <param name="notifier">The operation result notifier</param>
	public DefaultClientStatusProcessor(
		IRestClient client,
		IOperationResultNotifier notifier)
	{
		_client = client;
		_notifier = notifier;
	}

	/// <inheritdoc />
	public async Task<OperationResult<bool>> Process(T request)
	{
		var dtoType = request.GetType();
		var restEndpoint = dtoType.GetCustomAttribute<RestEndpointAttribute>();

		if (restEndpoint is null)
		{
			throw new InvalidOperationException($"DTO {dtoType.Name} does not define a [{nameof(RestEndpointAttribute)}]. You must either decorate {dtoType.Name} with a [{nameof(RestEndpointAttribute)}] or add a custom implementation of {nameof(IStatusProcessor<T>)}.");
		}

		if (restEndpoint.Method is null)
		{
			throw new InvalidOperationException($"DTO {dtoType.Name} does not specify a {nameof(RestEndpointAttribute.Method)}. You must either specify both constructor arguments of {nameof(RestEndpointAttribute)} or add a custom implementation of {nameof(IStatusProcessor<T>)}.");
		}

		var result = await _client.SendRequest<bool>(
			restEndpoint.Endpoint,
			request,
			restEndpoint.Method);

		return _notifier.HandleWebResult(result);
	}
}