using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Sienar.Hooks;

/// <summary>
/// Runs any type of after-action hook
/// </summary>
/// <typeparam name="THook">The type of the hook to run. Must inherit from <see cref="IAfterActionBase{T}">IAfterActionBase&lt;TTarget&gt;</see></typeparam>
/// <typeparam name="TTarget">The type of the after-action hook input</typeparam>
public class DefaultAfterActionRunner<THook, TTarget> : IAfterActionRunner<THook, TTarget>
	where THook : IAfterActionBase<TTarget>
{
	private readonly IEnumerable<THook> _hooks;
	private readonly ILogger<DefaultAfterActionRunner<THook, TTarget>> _logger;

	/// <summary>
	/// Creates a new instance of <c>DefaultAfterActionRunner</c>
	/// </summary>
	/// <param name="hooks">The hooks to run</param>
	/// <param name="logger">The logger</param>
	public DefaultAfterActionRunner(
		IEnumerable<THook> hooks,
		ILogger<DefaultAfterActionRunner<THook, TTarget>> logger)
	{
		_hooks = hooks;
		_logger = logger;
	}

	/// <inheritdoc />
	public async Task Run(TTarget input)
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
			}
		}
	}
}
