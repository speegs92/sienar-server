namespace Sienar.Data;

/// <summary>
/// Performs arbitrary actions before an update action has run
/// </summary>
/// <remarks>
/// When this hook is called, the instance of <c>T</c> has not yet been updated in the database, so exercise care when querying the database, adding related entities, or any other database-related work.
/// </remarks>
/// <typeparam name="T">The type of the entity</typeparam>
public interface IBeforeUpdateAction<T> : IBeforeActionBase<T>
	where T : EntityBase;
