namespace Sienar.Data;

/// <summary>
/// Performs arbitrary work after a create action has already run
/// </summary>
/// <remarks>
/// When this hook is called, the instance of <c>T</c> is guaranteed to exist in the database, so it is safe to query the database, add related entities, or any other database-related work.
/// </remarks>
/// <typeparam name="T">The type of the entity</typeparam>
public interface IAfterCreateAction<T> : IAfterActionBase<T>
	where T : IEntity;
