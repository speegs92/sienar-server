namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityDeleteActor{T}"/> which deletes entities from an EntityFramework <see cref="DbContext"/>
/// </summary>
/// <typeparam name="T">The type of the entity to delete</typeparam>
public class EfEntityDeleteActor<T> : IEntityDeleteActor<T>
	where T : EntityBase
{
	private readonly IDbContext _context;
	private readonly ILogger<EfEntityDeleteActor<T>> _logger;
	private readonly IAccessValidationRunner<T> _accessValidationRunner;
	private readonly IStateValidationRunner<T> _stateValidationRunner;
	private readonly IBeforeActionRunner<IBeforeDeleteAction<T>, T> _beforeActionRunner;
	private readonly IAfterActionRunner<IAfterDeleteAction<T>, T> _afterActionRunner;
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
	public EfEntityDeleteActor(
		IDbContext context,
		ILogger<EfEntityDeleteActor<T>> logger,
		IAccessValidationRunner<T> accessValidationRunner,
		IStateValidationRunner<T> stateValidationRunner,
		IBeforeActionRunner<IBeforeDeleteAction<T>, T> beforeActionRunner,
		IAfterActionRunner<IAfterDeleteAction<T>, T> afterActionRunner,
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
		T? entity;
		var entitySet = _context.Set<T>();

		try
		{
			entity = await entitySet.FindAsync(id);
			if (entity is null)
			{
				return _notifier.HandleOperationResult(new OperationResult<bool>(
					OperationStatus.NotFound,
					false,
					StatusMessages.Crud<T>.NotFound(id)));
			}
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				StatusMessages.Crud<T>.DeleteFailed()));
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
				StatusMessages.Crud<T>.NoPermission()));
		}

		// Run state validation
		var stateValidationResult = await _stateValidationRunner.Validate(entity, ActionType.Delete);
		if (!stateValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unprocessable,
				false,
				stateValidationResult.Message ?? StatusMessages.Crud<T>.DeleteFailed()));
		}

		// Run before hooks
		var beforeHooksResult = await _beforeActionRunner.Run(entity);
		if (!beforeHooksResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				beforeHooksResult.Message ?? StatusMessages.Crud<T>.DeleteFailed()));
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
				StatusMessages.Crud<T>.DeleteFailed()));
		}

		// Run after hooks
		await _afterActionRunner.Run(entity);

		return _notifier.HandleOperationResult(new OperationResult<bool>(
			OperationStatus.Success,
			true,
			StatusMessages.Crud<T>.DeleteSuccessful()));
	}
}
