namespace Sienar.Infrastructure;

/// <inheritdoc />
public class DefaultCreateActionOrchestrator<TDto, TEntity>
	: ICreateActionOrchestrator<TDto, TEntity>
	where TDto : class
	where TEntity : IEntity, new()
{
	private readonly IMapper<TDto, TEntity> _dtoToEntityMapper;
	private readonly IEntityCreateActor<TEntity> _entityCreator;
	private readonly IOperationResultMapper _resultMapper;

	/// <summary>
	/// Creates a new instance of <c>DefaultCreateActionOrchestrator</c>
	/// </summary>
	/// <param name="dtoToEntityMapper">The DTO-to-entity mapper</param>
	/// <param name="entityCreator">The entity create actor</param>
	/// <param name="resultMapper">The result mapper</param>
	public DefaultCreateActionOrchestrator(
		IMapper<TDto, TEntity> dtoToEntityMapper,
		IEntityCreateActor<TEntity> entityCreator,
		IOperationResultMapper resultMapper)
	{
		_dtoToEntityMapper = dtoToEntityMapper;
		_entityCreator = entityCreator;
		_resultMapper = resultMapper;
	}

	/// <inheritdoc />
	public async Task<IActionResult> Execute(TDto dto)
	{
		var entity = new TEntity();
		_dtoToEntityMapper.Map(dto, entity);
		var result = await _entityCreator.Create(entity);
		return _resultMapper.MapToObjectResult(result);
	}
}
