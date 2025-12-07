namespace Sienar.Data;

/// <summary>
/// Performs arbitrary actions before a delete action has run
/// </summary>
/// <typeparam name="T">The type of the entity</typeparam>
public interface IBeforeDeleteAction<T> : IBeforeActionBase<T>
	where T : EntityBase;
