namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityCreateActor{T}"/> which creates entities at a REST endpoint
/// </summary>
/// <typeparam name="T">The type of the DTO to create</typeparam>
public class RestEntityCreateActor<T> : IEntityCreateActor<T>
	where T : IEntity
{
	private readonly IRestClient _client;
	private readonly ICrudEndpointGenerator<T> _endpointGenerator;
	private readonly IOperationResultNotifier _notifier;
	private readonly ILogger<RestEntityCreateActor<T>> _logger;
	private readonly IAccessValidationRunner<T> _accessValidationRunner;
	private readonly IStateValidationRunner<T> _stateValidationRunner;
	private readonly IBeforeActionRunner<IBeforeCreateAction<T>, T> _beforeActionRunner;
	private readonly IAfterActionRunner<IAfterCreateAction<T>, T> _afterActionRunner;

	/// <summary>
	/// Creates a new instance of <c>RestEntityCreateActor</c>
	/// </summary>
	/// <param name="client">The rest client</param>
	/// <param name="endpointGenerator">The endpoint generator for the given DTO</param>
	/// <param name="notifier">The notifier</param>
	/// <param name="logger">The logger</param>
	/// <param name="accessValidationRunner">The access validation runner</param>
	/// <param name="stateValidationRunner">The state validation runner</param>
	/// <param name="beforeActionRunner">The before-action runner</param>
	/// <param name="afterActionRunner">The after-action runner</param>
	public RestEntityCreateActor(
		IRestClient client,
		ICrudEndpointGenerator<T> endpointGenerator,
		IOperationResultNotifier notifier,
		ILogger<RestEntityCreateActor<T>> logger,
		IAccessValidationRunner<T> accessValidationRunner,
		IStateValidationRunner<T> stateValidationRunner,
		IBeforeActionRunner<IBeforeCreateAction<T>, T> beforeActionRunner,
		IAfterActionRunner<IAfterCreateAction<T>, T> afterActionRunner)
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
	public async Task<OperationResult<int>> Create(T model)
	{
		// Run access validation
		var accessValidationResult = await _accessValidationRunner.Validate(
			model,
			ActionType.Create);
		if (!accessValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<int>(
				OperationStatus.Unauthorized,
				0,
				StatusMessages.Crud<T>.NoPermission()));
		}

		// Run state validation
		var stateValidationResult = await _stateValidationRunner.Validate(
			model,
			ActionType.Create);
		if (!stateValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<int>(
				OperationStatus.Unprocessable,
				0,
				stateValidationResult.Message ?? StatusMessages.Crud<T>.CreateFailed()));
		}

		// Run before hooks
		var beforeHooksResult = await _beforeActionRunner.Run(model);
		if (!beforeHooksResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<int>(
				OperationStatus.Unknown,
				0,
				beforeHooksResult.Message ?? StatusMessages.Crud<T>.CreateFailed()));
		}

		OperationResult<WebResult<int>> result;
		try
		{
			var endpoint = _endpointGenerator.GenerateCreateUrl(model);
			result = await _client.Post<int>(
				endpoint,
				model);
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return _notifier.HandleOperationResult(new OperationResult<int>(
				OperationStatus.Unknown,
				0,
				StatusMessages.Crud<T>.CreateFailed()));
		}

		// Run after hooks
		await _afterActionRunner.Run(model);

		return _notifier.HandleWebResult(result);
	}
}