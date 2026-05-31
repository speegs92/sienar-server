namespace Sienar.Data;

/// <summary>
/// A service to update instances of <c>T</c> in the appropriate repository
/// </summary>
/// <typeparam name="T">The type of the entity</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IEntityUpdateActor<T>
{
	/// <summary>
	/// Updates an existing DTO in the database
	/// </summary>
	/// <param name="model">The entity to update</param>
	/// <returns>Whether the edit operation was successful</returns>
	Task<OperationResult<bool>> Update(T model);
}