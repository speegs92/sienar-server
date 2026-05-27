#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Data;

/// <exclude />
public class ConcurrencyStampUpdater<TEntity> :
	IBeforeCreateAction<TEntity>,
	IBeforeUpdateAction<TEntity>
	where TEntity : IEntity
{
	public Task Handle(TEntity entity)
	{
		entity.ConcurrencyStamp = Guid.NewGuid();
		return Task.CompletedTask;
	}
}