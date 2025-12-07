using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <inheritdoc />
public class DefaultResultActionOrchestrator<T> :
	IResultActionOrchestrator<T>
	where T : IResult
{
	private readonly IResultActor<T> _actor;
	private readonly IOperationResultMapper _mapper;

	/// <summary>
	/// Creates a new instance of <c>DefaultResultActionOrchestrator</c>
	/// </summary>
	/// <param name="actor">The status actor</param>
	/// <param name="mapper">The result mapper</param>
	public DefaultResultActionOrchestrator(
		IResultActor<T> actor,
		IOperationResultMapper mapper)
	{
		_actor = actor;
		_mapper = mapper;
	}

	/// <inheritdoc />
	public async Task<IActionResult> Execute()
	{
		var result = await _actor.Execute();
		return _mapper.MapToObjectResult(result);
	}
}
