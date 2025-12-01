using System.Linq;

namespace Sienar.Data;

/// <summary>
/// Maps the properties of one type to another via reflection
/// </summary>
/// <typeparam name="TSource">The type of the source object</typeparam>
/// <typeparam name="TTarget">The type of the target object</typeparam>
public class DefaultMapper<TSource, TTarget> : IMapper<TSource, TTarget>
	where TSource : class
	where TTarget : class
{
	/// <inheritdoc />
	public void Map(TSource source, TTarget target)
	{
		var sourceProps = typeof(TSource).GetProperties();
		var targetProps = typeof(TTarget).GetProperties();

		foreach (var sourceProp in sourceProps)
		{
			targetProps
				.FirstOrDefault(p => p.Name == sourceProp.Name)?
				.SetValue(target, sourceProp.GetValue(source));
		}
	}
}
