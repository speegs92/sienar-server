// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

/// <summary>
/// A configured version of <see cref="MudTextField{T}"/>
/// </summary>
/// <typeparam name="T">The type of the input value</typeparam>
public class Textbox<T> : MudTextField<T>
{
	/// <inheritdoc />
	public Textbox()
	{
		Variant = Variant.Outlined;
		Immediate = true;
	}
}