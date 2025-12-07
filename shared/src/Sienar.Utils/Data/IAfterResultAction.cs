using Sienar.Hooks;

namespace Sienar.Data;

/// <summary>
/// Performs arbitrary work after a result action has already run
/// </summary>
/// <typeparam name="T">The type of the request</typeparam>
public interface IAfterResultAction<T> : IAfterActionBase<T>
	where T : IResult;
