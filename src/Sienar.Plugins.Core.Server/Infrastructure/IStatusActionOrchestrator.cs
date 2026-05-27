namespace Sienar.Infrastructure;

/// <summary>
/// Orchestrates and calls the services needed to respond to an HTTP request which accepts input and returns the success status
/// </summary>
/// <typeparam name="T">The type of the request</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IStatusActionOrchestrator<T>
	where T : IRequest
{
	/// <summary>
	/// Executes the orchestrated action
	/// </summary>
	/// <param name="request">The request</param>
	/// <returns>The final action result</returns>
	Task<IActionResult> Execute(T request);
}
