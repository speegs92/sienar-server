// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

/// <summary>
/// A configured version of <see cref="MudSelect{T}"/>
/// </summary>
/// <typeparam name="T">The type of the input value</typeparam>
public class Select<T> : MudSelect<T>
{
	/// <inheritdoc />
	public Select()
	{
		Variant = Variant.Outlined;
		AnchorOrigin = Origin.BottomLeft;
		TransformOrigin = Origin.TopLeft;
	}
}