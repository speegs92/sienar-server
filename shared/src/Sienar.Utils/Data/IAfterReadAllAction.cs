using Sienar.Hooks;

namespace Sienar.Data;

/// <summary>
/// Performs arbitrary work after a read-all action has already run
/// </summary>
/// <typeparam name="T">The type of the entity</typeparam>
public interface IAfterReadAllAction<T> : IAfterActionBase<T>
	where T : EntityBase;
