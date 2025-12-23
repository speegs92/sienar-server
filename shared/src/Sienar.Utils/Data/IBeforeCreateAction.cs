namespace Sienar.Data;

/// <summary>
/// Performs arbitrary actions before a create action has run
/// </summary>
/// <remarks>
/// When this hook is called, the instance of <c>T</c> has not yet been added to the database, so exercise care when querying the database, adding related entities, or any other database-related work.
/// </remarks>
/// <typeparam name="T">The type of the entity</typeparam>
public interface IBeforeCreateAction<T> : IBeforeActionBase<T>
	where T : IEntity;
