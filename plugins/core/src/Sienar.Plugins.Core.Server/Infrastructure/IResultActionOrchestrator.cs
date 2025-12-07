using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <summary>
/// Orchestrates and calls the services needed to respond to an HTTP request which accepts no input and returns output
/// </summary>
/// <typeparam name="T">The type of the result</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IResultActionOrchestrator<T>
	where T : IResult
{
	
	/// <summary>
	/// Executes the orchestrated action
	/// </summary>
	/// <returns>The final action result</returns>
	Task<IActionResult> Execute();
}
