namespace Sienar.Infrastructure;

/// <inheritdoc />
public class DefaultStatusActionOrchestrator<T> : IStatusActionOrchestrator<T>
	where T : IRequest
{
	private readonly IStatusActor<T> _actor;
	private readonly IOperationResultMapper _mapper;

	/// <summary>
	/// Creates a new instance of <c>DefaultStatusActionOrchestrator</c>
	/// </summary>
	/// <param name="actor">The status actor</param>
	/// <param name="mapper">The result mapper</param>
	public DefaultStatusActionOrchestrator(
		IStatusActor<T> actor,
		IOperationResultMapper mapper)
	{
		_actor = actor;
		_mapper = mapper;
	}

	/// <inheritdoc />
	public async Task<IActionResult> Execute(T request)
	{
		var result = await _actor.Execute(request);
		return _mapper.MapToObjectResult(result);
	}
}
