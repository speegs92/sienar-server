// ReSharper disable TypeParameterCanBeVariant
namespace Sienar.Data;

/// <summary>
/// Maps between two class instances
/// </summary>
/// <typeparam name="TSource">The type of the source class</typeparam>
/// <typeparam name="TTarget">The type of the target class</typeparam>
public interface IMapper<TSource, TTarget>
	where TSource : class
	where TTarget : class
{
	/// <summary>
	/// Maps a source class to a target class
	/// </summary>
	/// <param name="source">The object from which to map</param>
	/// <param name="target">The object onto which to map</param>
	void Map(TSource source, TTarget target);
}
