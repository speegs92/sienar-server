namespace Sienar.Data;

/// <summary>
/// Performs arbitrary work after a delete action has already run
/// </summary>
/// <typeparam name="T">The type of the entity</typeparam>
public interface IAfterDeleteAction<T> : IAfterActionBase<T>
	where T : EntityBase;
