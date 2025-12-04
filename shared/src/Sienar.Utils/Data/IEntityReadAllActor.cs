using System.Threading.Tasks;
using Sienar.Infrastructure;

namespace Sienar.Data;

/// <summary>
/// A service to read multiple instances of <c>T</c> from the appropriate repository
/// </summary>
/// <typeparam name="T">The type of the entity</typeparam>
public interface IEntityReadAllActor<T>
{
	/// <summary>
	/// Gets multiple entities
	/// </summary>
	/// <param name="filter">A <see cref="Filter"/> to specify included results</param>
	/// <returns>A list of all entities in the database</returns>
	Task<OperationResult<PagedQueryResult<T>>> Read(
		Filter? filter = null);
}