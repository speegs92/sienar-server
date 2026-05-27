namespace Sienar.Infrastructure;

/// <summary>
/// Orchestrates and calls the services needed to respond to a CRUD POST request 
/// </summary>
/// <typeparam name="TDto">The type of the DTO</typeparam>
/// <typeparam name="TEntity">The type of the entity</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface ICreateActionOrchestrator<TDto, TEntity>
	where TDto : class
	where TEntity : IEntity, new()
{
	/// <summary>
	/// Executes the orchestrated action
	/// </summary>
	/// <param name="dto">The type of the DTO</param>
	/// <returns>The final action result</returns>
	Task<IActionResult> Execute(TDto dto);
}
