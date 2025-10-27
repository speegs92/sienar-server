namespace Sienar.Themes;

/// <summary>
/// The solidity variants supported by Sienar
/// </summary>
public enum ThemeVariant
{
	/// <summary>
	/// A solid variant; generally filled with a single color and with an offset text color
	/// </summary>
	Solid,

	/// <summary>
	/// An outlined variant; generally similar to the <see cref="Solid"/> variant but with no color fill
	/// </summary>
	Outlined,

	/// <summary>
	/// A text-only variant; generally similar to the <see cref="Outlined"/> variant but with no outline
	/// </summary>
	Text
}
