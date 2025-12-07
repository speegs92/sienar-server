#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure;

namespace Sienar.Hooks;

/// <summary>
/// Runs any type of before-action hook
/// </summary>
/// <typeparam name="THook">The type of the hook to run. Must inherit from <see cref="IBeforeActionBase{T}">IBeforeActionBase&lt;TTarget&gt;</see></typeparam>
/// <typeparam name="TInput">The type of the after-action hook input</typeparam>
public class DefaultBeforeActionRunner<THook, TInput> : IBeforeActionRunner<THook, TInput>
	where THook : IBeforeActionBase<TInput>
{
	private readonly IEnumerable<THook> _hooks;
	private readonly ILogger<DefaultBeforeActionRunner<THook, TInput>> _logger;

	/// <summary>
	/// Creates a new instance of <c>DefaultBeforeActionRunner</c>
	/// </summary>
	/// <param name="hooks">The hooks to run</param>
	/// <param name="logger">The logger</param>
	public DefaultBeforeActionRunner(
		IEnumerable<THook> hooks,
		ILogger<DefaultBeforeActionRunner<THook, TInput>> logger)
	{
		_hooks = hooks;
		_logger = logger;
	}

	/// <inheritdoc />
	public async Task<OperationResult<bool>> Run(TInput input)
	{
		foreach (var hook in _hooks)
		{
			try
			{
				await hook.Handle(input);
			}
			catch (Exception e)
			{
				_logger.LogError(
					e,
					"{hookType} {hookFqcn} failed to run",
					typeof(THook),
					hook.GetType().FullName);
				return new(
					OperationStatus.Unknown,
					false,
					StatusMessages.Processes.BeforeHookFailure);
			}
		}

		return new(OperationStatus.Success, true);
	}
}
