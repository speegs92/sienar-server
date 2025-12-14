namespace Sienar.Data;

/// <summary>
/// A service to read single instances of <c>T</c> from the appropriate repository
/// </summary>
/// <typeparam name="T">The type of the entity</typeparam>
public interface IEntityReadActor<T>
{
	/// <summary>
	/// Gets an entity by primary key
	/// </summary>
	/// <param name="id">The primary key of the entity to retrieve</param>
	/// <param name="filter">A <see cref="Filter"/> to specify included results</param>
	/// <returns>the requested entity</returns>
	Task<OperationResult<T>> Read(
		int id,
		Filter? filter = null);
}