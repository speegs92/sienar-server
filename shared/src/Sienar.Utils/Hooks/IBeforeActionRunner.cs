using System.Threading.Tasks;
using Sienar.Infrastructure;

namespace Sienar.Hooks;

/// <summary>
/// Runs before-action hooks for a hookable request
/// </summary>
/// <typeparam name="THook">The type of the hook to run</typeparam>
/// <typeparam name="TInput">the type of the request or entity</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IBeforeActionRunner<THook, TInput>
	where THook : IBeforeActionBase<TInput>
{
	/// <summary>
	/// Runs all before-action hooks for a hookable request
	/// </summary>
	/// <param name="input">the request or entity</param>
	/// <returns>an operation result representing whether the hooks allow the process to continue</returns>
	Task<OperationResult<bool>> Run(TInput input);
}
