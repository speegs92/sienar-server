using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <summary>
/// Orchestrates and calls the services needed to respond to an HTTP request which accepts input and returns output
/// </summary>
/// <typeparam name="TRequest">The type of the request</typeparam>
/// <typeparam name="TResult">The type of the result</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IGeneralActionOrchestrator<TRequest, TResult>
	where TRequest : IRequest
	where TResult : IResult
{
	/// <summary>
	/// Executes the orchestrated action
	/// </summary>
	/// <param name="request">The request</param>
	/// <returns>The final action result</returns>
	Task<IActionResult> Execute(TRequest request);
}
