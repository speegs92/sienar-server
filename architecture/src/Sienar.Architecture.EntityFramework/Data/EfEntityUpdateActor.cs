using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Security;

namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityUpdateActor{T}"/> which updates entities in an EntityFramework <see cref="DbContext"/>
/// </summary>
/// <typeparam name="T">The type of the entity to update</typeparam>
public class EfEntityUpdateActor<T> : IEntityUpdateActor<T>
	where T : EntityBase
{
	private readonly IDbContext _context;
	private readonly ILogger<EfEntityUpdateActor<T>> _logger;
	private readonly IAccessValidationRunner<T> _accessValidationRunner;
	private readonly IStateValidationRunner<T> _stateValidationRunner;
	private readonly IBeforeActionRunner<T> _beforeActionRunner;
	private readonly IAfterActionRunner<T> _afterActionRunner;
	private readonly IOperationResultNotifier _notifier;

	/// <summary>
	/// Creates a new instance of <c>EfEntityUpdateActor</c>
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="logger">The logger</param>
	/// <param name="accessValidationRunner">The access validation runner</param>
	/// <param name="stateValidationRunner">The state validation runner</param>
	/// <param name="beforeActionRunner">The before-hook action runner</param>
	/// <param name="afterActionRunner">The after-hook action runner</param>
	/// <param name="notifier">The operation result notifier</param>
	public EfEntityUpdateActor(
		IDbContext context,
		ILogger<EfEntityUpdateActor<T>> logger,
		IAccessValidationRunner<T> accessValidationRunner,
		IStateValidationRunner<T> stateValidationRunner,
		IBeforeActionRunner<T> beforeActionRunner,
		IAfterActionRunner<T> afterActionRunner,
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
	public async Task<OperationResult<bool>> Update(T model)
	{
		// Run access validation
		var accessValidationResult = await _accessValidationRunner.Validate(
			model,
			ActionType.Update);
		if (!accessValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unauthorized,
				false,
				StatusMessages.Crud<T>.NoPermission()));
		}

		// Run state validation
		var stateValidationResult = await _stateValidationRunner.Validate(
			model,
			ActionType.Update);
		if (!stateValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unprocessable,
				false,
				stateValidationResult.Message ?? StatusMessages.Crud<T>.UpdateFailed()));
		}

		// Run before hooks
		var beforeHooksResult = await _beforeActionRunner.Run(model, ActionType.Update);
		if (!beforeHooksResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				beforeHooksResult.Message ?? StatusMessages.Crud<T>.UpdateFailed()));
		}

		try
		{
			_context
				.Set<T>()
				.Update(model);
			await _context.SaveChangesAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return _notifier.HandleOperationResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				StatusMessages.Crud<T>.UpdateFailed()));
		}

		// Run after hooks
		await _afterActionRunner.Run(model, ActionType.Update);

		return _notifier.HandleOperationResult(new OperationResult<bool>(
			OperationStatus.Success,
			true,
			StatusMessages.Crud<T>.UpdateSuccessful()));
	}
}