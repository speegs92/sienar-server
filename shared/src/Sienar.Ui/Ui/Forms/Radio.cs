// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

/// <summary>
/// A configured version of <see cref="MudRadio{T}"/>
/// </summary>
/// <typeparam name="T">The type of the input value</typeparam>
public class Radio<T> : MudRadio<T>
{
	/// <inheritdoc />
	public Radio()
	{
		Class = "d-flex";
		Color = Color.Primary;
	}
}