using Sienar.Hooks;

namespace Sienar.Data;

/// <summary>
/// Performs arbitrary actions before a general action has run
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IBeforeGeneralAction<T> : IBeforeActionBase<T>
	where T : IRequest;
