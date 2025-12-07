namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityReadActor{T}"/> which reads entities from a REST endpoint
/// </summary>
/// <typeparam name="T">The type of the DTO to read</typeparam>
public class RestEntityReadActor<T> : IEntityReadActor<T>
	where T : EntityBase
{
	private readonly IRestClient _client;
	private readonly ICrudEndpointGenerator<T> _endpointGenerator;
	private readonly IOperationResultNotifier _notifier;
	private readonly ILogger<RestEntityReadActor<T>> _logger;
	private readonly IAccessValidationRunner<T> _accessValidationRunner;
	private readonly IAfterActionRunner<IAfterReadAction<T>, T> _afterActionRunner;

	/// <summary>
	/// Creates a new instance of <c>RestEntityReadActor</c>
	/// </summary>
	/// <param name="client">The rest client</param>
	/// <param name="endpointGenerator">The endpoint generator for the given DTO</param>
	/// <param name="notifier">The operation result notifier</param>
	/// <param name="logger">The logger</param>
	/// <param name="accessValidationRunner">The access validation runner</param>
	/// <param name="afterActionRunner">The after-action runner</param>
	public RestEntityReadActor(
		IRestClient client,
		ICrudEndpointGenerator<T> endpointGenerator,
		IOperationResultNotifier notifier,
		ILogger<RestEntityReadActor<T>> logger,
		IAccessValidationRunner<T> accessValidationRunner,
		IAfterActionRunner<IAfterReadAction<T>, T> afterActionRunner)
	{
		_client = client;
		_endpointGenerator = endpointGenerator;
		_notifier = notifier;
		_logger = logger;
		_accessValidationRunner = accessValidationRunner;
		_afterActionRunner = afterActionRunner;
	}

	/// <inheritdoc />
	public async Task<OperationResult<T?>> Read(
		int id,
		Filter? filter = null)
	{
		T? entity;
		OperationResult<WebResult<T?>> result;

		try
		{
			var endpoint = _endpointGenerator.GenerateReadUrl(id);
			result = await _client.Get<T>(
				endpoint,
				filter);
			entity = result.Result?.Result;
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return _notifier.HandleOperationResult(new OperationResult<T?>(
				OperationStatus.Unknown,
				null,
				StatusMessages.Crud<T>.ReadSingleFailed()));
		}

		if (entity is null)
		{
			return _notifier.HandleOperationResult(new OperationResult<T?>(
				OperationStatus.NotFound,
				null,
				StatusMessages.Crud<T>.NotFound(id)));
		}

		// Run access validation
		var accessValidationResult = await _accessValidationRunner.Validate(entity, ActionType.Read);
		if (!accessValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<T?>(
				OperationStatus.Unauthorized,
				null,
				StatusMessages.Crud<T>.NoPermission()));
		}

		await _afterActionRunner.Run(entity);
		return _notifier.HandleWebResult(result);
	}
}