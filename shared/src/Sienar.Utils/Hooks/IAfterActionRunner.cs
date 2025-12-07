namespace Sienar.Hooks;

/// <summary>
/// Runs after-action hooks for a hookable request
/// </summary>
/// <typeparam name="THook">The type of the hook to run</typeparam>
/// <typeparam name="TTarget">the type of the request or entity</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IAfterActionRunner<THook, TTarget>
	where THook : IAfterActionBase<TTarget>
{
	/// <summary>
	///  Runs all after-action hooks for a hookable request
	/// </summary>
	/// <param name="input">the request or entity</param>
	Task Run(TTarget input);
}
