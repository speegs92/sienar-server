namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityUpdateActor{T}"/> which updates entities at a REST endpoint
/// </summary>
/// <typeparam name="T">The type of the DTO to update</typeparam>
public class RestEntityUpdateActor<T> : IEntityUpdateActor<T>
	where T : EntityBase
{
	private readonly IRestClient _client;
	private readonly ICrudEndpointGenerator<T> _endpointGenerator;
	private readonly IOperationResultNotifier _notifier;
	private readonly ILogger<RestEntityUpdateActor<T>> _logger;
	private readonly IAccessValidationRunner<T> _accessValidationRunner;
	private readonly IStateValidationRunner<T> _stateValidationRunner;
	private readonly IBeforeActionRunner<IBeforeUpdateAction<T>, T> _beforeActionRunner;
	private readonly IAfterActionRunner<IAfterUpdateAction<T>, T> _afterActionRunner;

	/// <summary>
	/// Creates a new instance of <c>RestEntityUpdateActor</c>
	/// </summary>
	/// <param name="client">The rest client</param>
	/// <param name="endpointGenerator">The endpoint generator for the given DTO</param>
	/// <param name="notifier">The notifier</param>
	/// <param name="logger">The logger</param>
	/// <param name="accessValidationRunner">The access validation runner</param>
	/// <param name="stateValidationRunner">The state validation runner</param>
	/// <param name="beforeActionRunner">The before-action runner</param>
	/// <param name="afterActionRunner">The after-action runner</param>
	public RestEntityUpdateActor(
		IRestClient client,
		ICrudEndpointGenerator<T> endpointGenerator,
		IOperationResultNotifier notifier,
		ILogger<RestEntityUpdateActor<T>> logger,
		IAccessValidationRunner<T> accessValidationRunner,
		IStateValidationRunner<T> stateValidationRunner,
		IBeforeActionRunner<IBeforeUpdateAction<T>, T> beforeActionRunner,
		IAfterActionRunner<IAfterUpdateAction<T>, T> afterActionRunner)
	{
		_client = client;
		_endpointGenerator = endpointGenerator;
		_notifier = notifier;
		_logger = logger;
		_accessValidationRunner = accessValidationRunner;
		_stateValidationRunner = stateValidationRunner;
		_beforeActionRunner = beforeActionRunner;
		_afterActionRunner = afterActionRunner;
	}

	

	/// <inheritdoc />
	public async Task<OperationResult<bool>> Update(T model)
	{
		// Run access validation
		var accessValidationResult = await _accessValidationRunner.Validate(
			model,
			ActionType.Update);
		if (!accessValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unauthorized,
				false,
				StatusMessages.Crud<T>.NoPermission()));
		}

		// Run state validation
		var stateValidationResult = await _stateValidationRunner.Validate(
			model,
			ActionType.Update);
		if (!stateValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unprocessable,
				false,
				stateValidationResult.Message ?? StatusMessages.Crud<T>.UpdateFailed()));
		}

		// Run before hooks
		var beforeHooksResult = await _beforeActionRunner.Run(model);
		if (!beforeHooksResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				beforeHooksResult.Message ?? StatusMessages.Crud<T>.UpdateFailed()));
		}

		OperationResult<WebResult<bool>> result;
		try
		{
			var endpoint = _endpointGenerator.GenerateUpdateUrl(model);
			result = await _client.Put<bool>(
				endpoint,
				model);
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				StatusMessages.Crud<T>.UpdateFailed()));
		}

		// Run after hooks
		await _afterActionRunner.Run(model);

		return _notifier.HandleWebResult(result);
	}
}