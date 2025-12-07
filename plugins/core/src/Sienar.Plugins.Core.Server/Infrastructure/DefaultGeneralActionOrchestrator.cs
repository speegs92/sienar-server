using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <inheritdoc />
public class DefaultGeneralActionOrchestrator<TRequest, TResult> :
	IGeneralActionOrchestrator<TRequest, TResult>
	where TRequest : IRequest
	where TResult : IResult
{
	private readonly IGeneralActor<TRequest, TResult> _actor;
	private readonly IOperationResultMapper _mapper;

	/// <summary>
	/// Creates a new instance of <c>DefaultGeneralActionOrchestrator</c>
	/// </summary>
	/// <param name="actor">The general actor</param>
	/// <param name="mapper">The result mapper</param>
	public DefaultGeneralActionOrchestrator(
		IGeneralActor<TRequest, TResult> actor,
		IOperationResultMapper mapper)
	{
		_actor = actor;
		_mapper = mapper;
	}

	/// <inheritdoc />
	public async Task<IActionResult> Execute(TRequest request)
	{
		var result = await _actor.Execute(request);
		return _mapper.MapToObjectResult(result);
	}
}
