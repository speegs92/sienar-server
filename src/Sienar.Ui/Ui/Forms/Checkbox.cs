// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

/// <summary>
/// A configured version of <see cref="MudCheckBox{T}"/>
/// </summary>
/// <typeparam name="T">The type of the input value</typeparam>
public class Checkbox<T> : MudCheckBox<T>
{
	/// <inheritdoc />
	public Checkbox()
	{
		Color = Color.Primary;
		UncheckedColor = Color.Primary;
	}
}