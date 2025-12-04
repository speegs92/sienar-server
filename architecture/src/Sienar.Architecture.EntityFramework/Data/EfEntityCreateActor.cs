using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Security;

namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityCreateActor{T}"/> which creates entities in an EntityFramework <see cref="DbContext"/>
/// </summary>
/// <typeparam name="T">The type of the entity to create</typeparam>
public class EfEntityCreateActor<T> : IEntityCreateActor<T>
	where T : EntityBase
{
	private readonly IDbContext _context;
	private readonly ILogger<EfEntityCreateActor<T>> _logger;
	private readonly IAccessValidationRunner<T> _accessValidationRunner;
	private readonly IStateValidationRunner<T> _stateValidationRunner;
	private readonly IBeforeActionRunner<T> _beforeActionRunner;
	private readonly IAfterActionRunner<T> _afterActionRunner;
	private readonly IOperationResultNotifier _notifier;

	/// <summary>
	/// Creates a new instance of <c>EfEntityCreateActor</c>
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="logger">The logger</param>
	/// <param name="accessValidationRunner">The access validation runner</param>
	/// <param name="stateValidationRunner">The state validation runner</param>
	/// <param name="beforeActionRunner">The before-hook action runner</param>
	/// <param name="afterActionRunner">The after-hook action runner</param>
	/// <param name="notifier">The operation result notifier</param>
	public EfEntityCreateActor(
		IDbContext context,
		ILogger<EfEntityCreateActor<T>> logger,
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
	public async Task<OperationResult<int?>> Create(T model)
	{
		// Run access validation
		var accessValidationResult = await _accessValidationRunner.Validate(
			model,
			ActionType.Create);
		if (!accessValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<int?>(
				OperationStatus.Unauthorized,
				null,
				StatusMessages.Crud<T>.NoPermission()));
		}

		// Run state validation
		var stateValidationResult = await _stateValidationRunner.Validate(
			model,
			ActionType.Create);
		if (!stateValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<int?>(
				OperationStatus.Unprocessable,
				null,
				stateValidationResult.Message ?? StatusMessages.Crud<T>.CreateFailed()));
		}

		// Run before hooks
		var beforeHooksResult = await _beforeActionRunner.Run(model, ActionType.Create);
		if (!beforeHooksResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<int?>(
				OperationStatus.Unknown,
				null,
				beforeHooksResult.Message ?? StatusMessages.Crud<T>.CreateFailed()));
		}

		try
		{
			await _context
				.Set<T>()
				.AddAsync(model);
			await _context.SaveChangesAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return _notifier.HandleOperationResult(new OperationResult<int?>(
				OperationStatus.Unknown,
				null,
				StatusMessages.Crud<T>.CreateFailed()));
		}

		// Run after hooks
		await _afterActionRunner.Run(model, ActionType.Create);

		return _notifier.HandleOperationResult(new OperationResult<int?>(
			OperationStatus.Success,
			model.Id,
			StatusMessages.Crud<T>.CreateSuccessful()));
	}
}