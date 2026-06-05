#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Data;

/// <exclude />
public class DefaultGeneralActor<TRequest, TResult> : IGeneralActor<TRequest, TResult>
	where TRequest : IRequest
	where TResult : IResult
{
	private readonly ILogger<DefaultGeneralActor<TRequest, TResult>> _logger;
	private readonly IBotDetector _botDetector;
	private readonly IAccessValidationRunner<TRequest> _accessValidator;
	private readonly IStateValidationRunner<TRequest> _stateValidator;
	private readonly IBeforeActionRunner<IBeforeGeneralAction<TRequest>, TRequest> _beforeHooks;
	private readonly IAfterActionRunner<IAfterGeneralAction<TRequest>, TRequest> _afterHooks;
	private readonly IGeneralProcessor<TRequest, TResult> _processor;
	private readonly IOperationResultNotifier _notifier;

	public DefaultGeneralActor(
		ILogger<DefaultGeneralActor<TRequest, TResult>> logger,
		IBotDetector botDetector,
		IAccessValidationRunner<TRequest> accessValidator,
		IStateValidationRunner<TRequest> stateValidator,
		IBeforeActionRunner<IBeforeGeneralAction<TRequest>, TRequest> beforeHooks,
		IAfterActionRunner<IAfterGeneralAction<TRequest>, TRequest> afterHooks,
		IGeneralProcessor<TRequest, TResult> processor,
		IOperationResultNotifier notifier)
	{
		_logger = logger;
		_botDetector = botDetector;
		_accessValidator = accessValidator;
		_stateValidator = stateValidator;
		_beforeHooks = beforeHooks;
		_afterHooks = afterHooks;
		_processor = processor;
		_notifier = notifier;
	}

	public virtual async Task<OperationResult<TResult>> Execute(TRequest request)
	{
		if (request is HoneypotDto honeypot && _botDetector.IsSpambot(honeypot))
		{
			return _notifier.HandleOperationResult(new OperationResult<TResult>());
		}

		// Run access validation
		var accessValidationResult = await _accessValidator.Validate(request, ActionType.Action);
		if (!accessValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<TResult>(
				accessValidationResult.Status,
				default,
				accessValidationResult.Message));
		}

		// Run state validation
		var stateValidationResult = await _stateValidator.Validate(request, ActionType.Action);
		if (!stateValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<TResult>(
				stateValidationResult.Status,
				default,
				stateValidationResult.Message));
		}

		// Run before hooks
		var beforeHooksResult = await _beforeHooks.Run(request);
		if (!beforeHooksResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<TResult>(
				beforeHooksResult.Status,
				default,
				beforeHooksResult.Message));
		}

		OperationResult<TResult> result;
		try
		{
			result = await _processor.Process(request);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "{type} failed to process", typeof(IGeneralProcessor<TRequest, TResult>));
			return _notifier.HandleOperationResult(new OperationResult<TResult>(OperationStatus.Unknown));
		}

		if (result.Status is OperationStatus.Success)
		{
			await _afterHooks.Run(request);
		}

		return _notifier.HandleOperationResult(result);
	}
}