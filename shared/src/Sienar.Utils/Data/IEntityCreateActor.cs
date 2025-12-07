using System.Threading.Tasks;
using Sienar.Infrastructure;

namespace Sienar.Data;

/// <summary>
/// A service to create instances of <c>T</c> in the appropriate repository
/// </summary>
/// <typeparam name="T">The type of the entity</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IEntityCreateActor<T>
{
	/// <summary>
	/// Creates a new entry in the repository
	/// </summary>
	/// <param name="model">The entity to create</param>
	/// <returns>The entity's new primary key</returns>
	Task<OperationResult<int?>> Create(T model);
}