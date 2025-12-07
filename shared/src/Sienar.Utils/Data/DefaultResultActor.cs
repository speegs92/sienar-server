#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Security;

namespace Sienar.Data;

/// <exclude />
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class DefaultResultActor<TResult> : IResultActor<TResult>
	where TResult : IResult
{
	private readonly ILogger<DefaultResultActor<TResult>> _logger;
	private readonly IAccessValidationRunner<TResult> _accessValidator;
	private readonly IAfterActionRunner<IAfterResultAction<TResult>, TResult> _afterHooks;
	private readonly IResultProcessor<TResult> _processor;
	private readonly IOperationResultNotifier _notifier;

	public DefaultResultActor(
		ILogger<DefaultResultActor<TResult>> logger,
		IAccessValidationRunner<TResult> accessValidator,
		IAfterActionRunner<IAfterResultAction<TResult>, TResult> afterHooks,
		IResultProcessor<TResult> processor,
		IOperationResultNotifier notifier)
	{
		_logger = logger;
		_accessValidator = accessValidator;
		_afterHooks = afterHooks;
		_processor = processor;
		_notifier = notifier;
	}

	public virtual async Task<OperationResult<TResult?>> Execute()
	{
		// Run access validation
		var accessValidationResult = await _accessValidator.Validate(default, ActionType.Result);
		if (!accessValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<TResult?>(
				accessValidationResult.Status,
				default,
				accessValidationResult.Message));
		}

		OperationResult<TResult?> result;
		try
		{
			result = await _processor.Process();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "{type} failed to process", typeof(IResultProcessor<TResult>));
			return _notifier.HandleOperationResult(new OperationResult<TResult?>(OperationStatus.Unknown));
		}

		if (result.Status is OperationStatus.Success && result.Result is not null)
		{
			await _afterHooks.Run(result.Result);
		}

		return _notifier.HandleOperationResult(result);
	}
}