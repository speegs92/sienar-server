using Sienar.Hooks;

namespace Sienar.Data;

/// <summary>
/// Performs arbitrary actions before a general action has run
/// </summary>
/// <typeparam name="T">The type of the request</typeparam>
public interface IBeforeGeneralAction<T> : IBeforeActionBase<T>
	where T : IRequest;
