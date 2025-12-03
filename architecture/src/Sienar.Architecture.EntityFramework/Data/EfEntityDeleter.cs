using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Security;

namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityDeleter{TEntity}"/> which deletes entities from an EntityFramework <see cref="DbContext"/>
/// </summary>
/// <typeparam name="TEntity">The type of the entity to delete</typeparam>
public class EfEntityDeleter<TEntity> : IEntityDeleter<TEntity>
	where TEntity : EntityBase
{
	private readonly IDbContext _context;
	private readonly ILogger<EfEntityDeleter<TEntity>> _logger;
	private readonly IAccessValidationRunner<TEntity> _accessValidationRunner;
	private readonly IStateValidationRunner<TEntity> _stateValidationRunner;
	private readonly IBeforeActionRunner<TEntity> _beforeActionRunner;
	private readonly IAfterActionRunner<TEntity> _afterActionRunner;
	private readonly IOperationResultNotifier _notifier;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="logger">The logger</param>
	/// <param name="accessValidationRunner">The access validation runner</param>
	/// <param name="stateValidationRunner">The state validation runner</param>
	/// <param name="beforeActionRunner">The before-hook action runner</param>
	/// <param name="afterActionRunner">The after-hook action runner</param>
	/// <param name="notifier">The operation result notifier</param>
	public EfEntityDeleter(
		IDbContext context,
		ILogger<EfEntityDeleter<TEntity>> logger,
		IAccessValidationRunner<TEntity> accessValidationRunner,
		IStateValidationRunner<TEntity> stateValidationRunner,
		IBeforeActionRunner<TEntity> beforeActionRunner,
		IAfterActionRunner<TEntity> afterActionRunner,
		IOperationResultNotifier notifier)
	{
		_context = context;
		_logger = logger;
		_accessValidationRunner = accessValidationRunner;
		_stateValidationRunner = stateValidationRunner;
		_beforeActionRunner = beforeActionRunner;
		_afterActionRunner = afterActionRunner;
		_notifier = notifier;
	}

	/// <inheritdoc />
	public async Task<OperationResult<bool>> Delete(int id)
	{
		TEntity? entity;
		var entitySet = _context.Set<TEntity>();

		try
		{
			entity = await entitySet.FindAsync(id);
			if (entity is null)
			{
				return _notifier.HandleOperationResult(new OperationResult<bool>(
					OperationStatus.NotFound,
					false,
					StatusMessages.Crud<TEntity>.NotFound(id)));
			}
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				StatusMessages.Crud<TEntity>.DeleteFailed()));
		}

		// Run access validation
		var accessValidationResult = await _accessValidationRunner.Validate(
			entity,
			ActionType.Delete);
		if (!accessValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unauthorized,
				false,
				StatusMessages.Crud<TEntity>.NoPermission()));
		}

		// Run state validation
		var stateValidationResult = await _stateValidationRunner.Validate(entity, ActionType.Delete);
		if (!stateValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unprocessable,
				false,
				stateValidationResult.Message ?? StatusMessages.Crud<TEntity>.DeleteFailed()));
		}

		// Run before hooks
		var beforeHooksResult = await _beforeActionRunner.Run(
			entity,
			ActionType.Delete);
		if (!beforeHooksResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				beforeHooksResult.Message ?? StatusMessages.Crud<TEntity>.DeleteFailed()));
		}

		try
		{
			entitySet.Remove(entity);
			await _context.SaveChangesAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				StatusMessages.Crud<TEntity>.DeleteFailed()));
		}

		// Run after hooks
		await _afterActionRunner.Run(entity, ActionType.Delete);

		return _notifier.HandleOperationResult(new OperationResult<bool>(
			OperationStatus.Success,
			true,
			StatusMessages.Crud<TEntity>.DeleteSuccessful()));
	}
}
