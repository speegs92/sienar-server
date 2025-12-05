using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure;

namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityDeleteActor{T}"/> which deletes entities from a REST endpoint
/// </summary>
/// <typeparam name="T">The type of the entity to write</typeparam>
public class RestEntityDeleteActor<T> : IEntityDeleteActor<T>
	where T : EntityBase
{
	private readonly IRestClient _client;
	private readonly IOperationResultNotifier _notifier;
	private readonly ICrudEndpointGenerator<T> _endpointGenerator;
	private readonly ILogger<RestEntityDeleteActor<T>> _logger;

	/// <summary>
	/// Creates a new instance of <c>RestEntityDeleter</c>
	/// </summary>
	/// <param name="client">The rest client</param>
	/// <param name="notifier">The operation result notifier</param>
	/// <param name="endpointGenerator">The endpoint generator for the given DTO</param>
	/// <param name="logger">The logger</param>
	public RestEntityDeleteActor(
		IRestClient client,
		IOperationResultNotifier notifier,
		ICrudEndpointGenerator<T> endpointGenerator,
		ILogger<RestEntityDeleteActor<T>> logger)
	{
		_client = client;
		_notifier = notifier;
		_endpointGenerator = endpointGenerator;
		_logger = logger;
	}

	/// <inheritdoc />
	public async Task<OperationResult<bool>> Delete(int id)
	{
		try
		{
			var endpoint = _endpointGenerator.GenerateDeleteUrl(id);
			var result = await _client.Delete<bool>(endpoint);
			return _notifier.HandleWebResult(result);
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				StatusMessages.Crud<T>.DeleteFailed()));
		}
	}
}
