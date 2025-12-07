using System.Threading.Tasks;

namespace Sienar.Hooks;

/// <summary>
/// Performs arbitrary work after an action has already run
/// </summary>
/// <typeparam name="T">The type being acted upon</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IAfterActionBase<T>
{
	/// <summary>
	/// Performs arbitrary work after an action has already run
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	Task Handle(T input);
}
