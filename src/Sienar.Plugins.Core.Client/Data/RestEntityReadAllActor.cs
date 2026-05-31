namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityReadAllActor{T}"/> which reads entities from a REST endpoint
/// </summary>
/// <typeparam name="T">The type of the DTO to read</typeparam>
public class RestEntityReadAllActor<T> : IEntityReadAllActor<T>
	where T : IEntity
{
	private readonly IRestClient _client;
	private readonly ICrudEndpointGenerator<T> _endpointGenerator;
	private readonly IOperationResultNotifier _notifier;
	private readonly ILogger<RestEntityReadAllActor<T>> _logger;
	private readonly IAfterActionRunner<IAfterReadAllAction<T>, T> _afterActionRunner;

	/// <summary>
	/// Creates a new instance of <c>RestEntityReadAllActor</c>
	/// </summary>
	/// <param name="client">The rest client</param>
	/// <param name="endpointGenerator">The endpoint generator for the given DTO</param>
	/// <param name="notifier">The operation result notifier</param>
	/// <param name="logger">The logger</param>
	/// <param name="afterActionRunner">The after-action runner</param>
	public RestEntityReadAllActor(
		IRestClient client,
		ICrudEndpointGenerator<T> endpointGenerator,
		IOperationResultNotifier notifier,
		ILogger<RestEntityReadAllActor<T>> logger,
		IAfterActionRunner<IAfterReadAllAction<T>, T> afterActionRunner)
	{
		_client = client;
		_endpointGenerator = endpointGenerator;
		_notifier = notifier;
		_logger = logger;
		_afterActionRunner = afterActionRunner;
	}

	/// <inheritdoc />
	public async Task<OperationResult<PagedQueryResult<T>>> Read(Filter? filter = null)
	{
		OperationResult<WebResult<PagedQueryResult<T>>> webResult;
		PagedQueryResult<T> queryResult;

		try
		{
			var endpoint = _endpointGenerator.GenerateReadUrl();
			webResult = await _client.Get<PagedQueryResult<T>>(
				endpoint,
				filter);
			queryResult = webResult.Result?.Result ?? new PagedQueryResult<T>();
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return _notifier.HandleOperationResult(new OperationResult<PagedQueryResult<T>>(
				OperationStatus.Unknown,
				new PagedQueryResult<T>(),
				StatusMessages.Crud<T>.ReadMultipleFailed()));
		}

		foreach (var entity in queryResult.Items)
		{
			await _afterActionRunner.Run(entity);
		}

		return _notifier.HandleWebResult(webResult);
	}
}