using Sienar.Hooks;

namespace Sienar.Data;

/// <summary>
/// Performs arbitrary work after a general action has already run
/// </summary>
/// <typeparam name="T">The type of the request</typeparam>
public interface IAfterGeneralAction<T> : IAfterActionBase<T>
	where T : IRequest;
