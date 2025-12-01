using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <inheritdoc />
public class DefaultCreateActionOrchestrator<TDto, TEntity>
	: ICreateActionOrchestrator<TDto, TEntity>
	where TDto : class
	where TEntity : EntityBase, new()
{
	private readonly IMapper<TDto, TEntity> _dtoToEntityMapper;
	private readonly IEntityWriter<TEntity> _entityWriter;
	private readonly IOperationResultMapper _resultMapper;

	/// <summary>
	/// Creates a new instance of <c>DefaultCreateActionOrchestrator</c>
	/// </summary>
	/// <param name="dtoToEntityMapper">The DTO-to-entity mapper</param>
	/// <param name="entityWriter">The entity writer</param>
	/// <param name="resultMapper">The result mapper</param>
	public DefaultCreateActionOrchestrator(
		IMapper<TDto, TEntity> dtoToEntityMapper,
		IEntityWriter<TEntity> entityWriter,
		IOperationResultMapper resultMapper)
	{
		_dtoToEntityMapper = dtoToEntityMapper;
		_entityWriter = entityWriter;
		_resultMapper = resultMapper;
	}

	/// <inheritdoc />
	public async Task<IActionResult> Execute(TDto dto)
	{
		var entity = new TEntity();
		_dtoToEntityMapper.Map(dto, entity);
		var result = await _entityWriter.Create(entity);
		return _resultMapper.MapToObjectResult(result);
	}
}
