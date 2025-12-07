namespace Sienar.Infrastructure;

/// <inheritdoc />
public class DefaultReadAllActionOrchestrator<TDto, TEntity> : IReadAllActionOrchestrator<TDto, TEntity>
	where TDto : EntityBase, new()
	where TEntity : EntityBase
{
	private readonly IMapper<TEntity, TDto> _entityToDtoMapper;
	private readonly IEntityReadAllActor<TEntity> _entityReader;
	private readonly IOperationResultMapper _resultMapper;

	/// <summary>
	/// Creates a new <c>DefaultReadAllActionOrchestrator</c>
	/// </summary>
	/// <param name="entityToDtoMapper">The entity-to-DTO mapper</param>
	/// <param name="entityReader">The entity read-all actor</param>
	/// <param name="resultMapper">The result mapper</param>
	public DefaultReadAllActionOrchestrator(
		IMapper<TEntity, TDto> entityToDtoMapper,
		IEntityReadAllActor<TEntity> entityReader,
		IOperationResultMapper resultMapper)
	{
		_entityToDtoMapper = entityToDtoMapper;
		_entityReader = entityReader;
		_resultMapper = resultMapper;
	}

	/// <inheritdoc />
	public async Task<IActionResult> Execute(Filter? filter)
	{
		var result = await _entityReader.Read(filter);

		if (result.Status is not OperationStatus.Success)
		{
			return _resultMapper.MapToObjectResult(result);
		}

		var mappedDtos = new List<TDto>();

		foreach (var entity in result.Result!.Items)
		{
			var dto = new TDto();
			_entityToDtoMapper.Map(entity, dto);
			mappedDtos.Add(dto);
		}

		return _resultMapper.MapToObjectResult(
			new OperationResult<PagedQueryResult<TDto>>(
				result.Status,
				new()
				{
					Items = mappedDtos,
					TotalCount = result.Result!.TotalCount
				},
				result.Message));
	}
}