namespace Sienar.Infrastructure;

/// <inheritdoc />
public class DefaultReadActionOrchestrator<TDto, TEntity>
	: IReadActionOrchestrator<TDto, TEntity>
	where TDto : EntityBase, new()
	where TEntity : EntityBase
{
	private readonly IMapper<TEntity, TDto> _entityToDtoMapper;
	private readonly IEntityReadActor<TEntity> _entityReader;
	private readonly IOperationResultMapper _resultMapper;

	/// <summary>
	/// Creates a new instance of <c>DefaultReadActionOrchestrator</c>
	/// </summary>
	/// <param name="entityToDtoMapper">The entity-to-DTO mapper</param>
	/// <param name="entityReader">The entity read actor</param>
	/// <param name="resultMapper">The result mapper</param>
	public DefaultReadActionOrchestrator(
		IMapper<TEntity, TDto> entityToDtoMapper,
		IEntityReadActor<TEntity> entityReader,
		IOperationResultMapper resultMapper)
	{
		_entityToDtoMapper = entityToDtoMapper;
		_entityReader = entityReader;
		_resultMapper = resultMapper;
	}

	/// <inheritdoc />
	public async Task<IActionResult> Execute(int id, Filter? filter)
	{
		var dto = new TDto();
		var result = await _entityReader.Read(id, filter);

		if (result.Status is not OperationStatus.Success ||
			result.Result is null)
		{
			return _resultMapper.MapToObjectResult(result);
		}

		_entityToDtoMapper.Map(result.Result, dto);

		return _resultMapper.MapToObjectResult(new OperationResult<TDto>(
			result.Status,
			dto,
			result.Message));
	}
}
