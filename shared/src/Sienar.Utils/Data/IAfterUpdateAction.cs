namespace Sienar.Data;

/// <summary>
/// Performs arbitrary work after an update action has already run
/// </summary>
/// <remarks>
/// When this hook is called, the instance of <c>T</c> is guaranteed to have been updated in the database, so it is safe to query the database, add related entities, or any other database-related work.
/// </remarks>
/// <typeparam name="T">The type of the entity</typeparam>
public interface IAfterUpdateAction<T> : IAfterActionBase<T>
	where T : IEntity;
