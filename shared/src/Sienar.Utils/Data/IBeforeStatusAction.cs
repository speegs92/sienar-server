using Sienar.Hooks;

namespace Sienar.Data;

/// <summary>
/// Performs arbitrary actions before a status action has run
/// </summary>
/// <typeparam name="T">The type of the request</typeparam>
public interface IBeforeStatusAction<T> : IBeforeActionBase<T>
	where T : IRequest;
