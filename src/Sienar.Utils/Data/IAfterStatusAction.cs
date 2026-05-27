namespace Sienar.Data;

/// <summary>
/// Performs arbitrary work after a general action has already run
/// </summary>
/// <typeparam name="T">The type of the request</typeparam>
public interface IAfterStatusAction<T> : IAfterActionBase<T>
	where T : IRequest;
