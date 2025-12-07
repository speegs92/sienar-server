using Microsoft.EntityFrameworkCore;

namespace Sienar.Infrastructure;

/// <inheritdoc />
public class DefaultUpdateActionOrchestrator<TDto, TEntity> : IUpdateActionOrchestrator<TDto, TEntity>
	where TDto : EntityBase
	where TEntity : EntityBase, new()
{
	private readonly IDbContext _context;
	private readonly IMapper<TDto, TEntity> _dtoToEntityMapper;
	private readonly IEntityUpdateActor<TEntity> _entityUpdater;
	private readonly IOperationResultMapper _resultMapper;

	/// <summary>
	/// Creates a new instance of <c>DefaultUpdateActionOrchestrator</c>
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="dtoToEntityMapper">The DTO-to-entity mapper</param>
	/// <param name="entityUpdater">The entity update actor</param>
	/// <param name="resultMapper">The result mapper</param>
	public DefaultUpdateActionOrchestrator(
		IDbContext context,
		IMapper<TDto, TEntity> dtoToEntityMapper,
		IEntityUpdateActor<TEntity> entityUpdater,
		IOperationResultMapper resultMapper)
	{
		_context = context;
		_dtoToEntityMapper = dtoToEntityMapper;
		_entityUpdater = entityUpdater;
		_resultMapper = resultMapper;
	}

	/// <inheritdoc />
	public async Task<IActionResult> Execute(TDto dto)
	{
		var entity = await _context
			.Set<TEntity>()
			.FindAsync(dto.Id) ?? new TEntity();
		_dtoToEntityMapper.Map(dto, entity);
		var result = await _entityUpdater.Update(entity);
		return _resultMapper.MapToObjectResult(result);
	}
}