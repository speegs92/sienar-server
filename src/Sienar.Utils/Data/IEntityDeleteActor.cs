namespace Sienar.Data;

/// <summary>
/// A service to delete instances of <c>T</c> from the appropriate repository
/// </summary>
/// <typeparam name="T">the type of the entity</typeparam>
public interface IEntityDeleteActor<T>
{
	/// <summary>
	/// Deletes an entity by primary key
	/// </summary>
	/// <param name="id">The primary key of the entity to delete</param>
	/// <returns>whether the delete operation was successful</returns>
	Task<OperationResult<bool>> Delete(int id);
}