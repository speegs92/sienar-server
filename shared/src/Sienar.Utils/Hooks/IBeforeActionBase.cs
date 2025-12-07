using System.Threading.Tasks;

namespace Sienar.Hooks;

/// <summary>
/// Performs arbitrary actions before a processor has run
/// </summary>
/// <typeparam name="T">The type of the request or entity</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IBeforeActionBase<T>
{
	/// <summary>
	/// Performs arbitrary actions before a processor has run
	/// </summary>
	/// <param name="request">The request or entity</param>
	Task Handle(T request);
}