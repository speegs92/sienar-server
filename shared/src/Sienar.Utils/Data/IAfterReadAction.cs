namespace Sienar.Data;

/// <summary>
/// Performs arbitrary work after a read action has already run
/// </summary>
/// <typeparam name="T">The type of the entity</typeparam>
public interface IAfterReadAction<T> : IAfterActionBase<T>
	where T : IEntity;
