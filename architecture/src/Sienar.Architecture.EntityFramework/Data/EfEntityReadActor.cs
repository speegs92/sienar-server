namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityReadActor{T}"/> which reads entities from an EntityFramework <see cref="DbContext"/>
/// </summary>
/// <typeparam name="T">The type of the entity to read</typeparam>
public class EfEntityReadActor<T> : IEntityReadActor<T>
	where T : EntityBase
{
	private readonly IDbContext _context;
	private readonly IEfFilterProcessor<T> _filterProcessor;
	private readonly ILogger<EfEntityReadActor<T>> _logger;
	private readonly IAccessValidationRunner<T> _accessValidationRunner;
	private readonly IAfterActionRunner<IAfterReadAction<T>, T> _afterActionRunner;
	private readonly IOperationResultNotifier _notifier;

	/// <summary>
	/// Creates a new instance of <c>EfEntityReadActor</c>
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="filterProcessor">The EF filter processor</param>
	/// <param name="logger">The logger</param>
	/// <param name="accessValidationRunner">The access validation runner</param>
	/// <param name="afterActionRunner">The after-hook action runner</param>
	/// <param name="notifier">The operation result notifier</param>
	public EfEntityReadActor(
		IDbContext context,
		IEfFilterProcessor<T> filterProcessor,
		ILogger<EfEntityReadActor<T>> logger,
		IAccessValidationRunner<T> accessValidationRunner,
		IAfterActionRunner<IAfterReadAction<T>, T> afterActionRunner,
		IOperationResultNotifier notifier)
	{
		_context = context;
		_filterProcessor = filterProcessor;
		_logger = logger;
		_accessValidationRunner = accessValidationRunner;
		_afterActionRunner = afterActionRunner;
		_notifier = notifier;
	}
	
	/// <inheritdoc />
	public async Task<OperationResult<T?>> Read(
		int id,
		Filter? filter = null)
	{
		T? entity;
		var entitySet = _context.Set<T>();
		filter = _filterProcessor.ModifyFilter(filter, ActionType.Read);

		try
		{
			entity = filter is null
				? await entitySet.FindAsync(id)
				: await _filterProcessor
					.ProcessIncludes(entitySet, filter)
					.FirstOrDefaultAsync(e => e.Id == id);
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return _notifier.HandleOperationResult(new OperationResult<T?>(
				OperationStatus.Unknown,
				null,
				StatusMessages.Crud<T>.ReadSingleFailed()));
		}

		if (entity is null)
		{
			return _notifier.HandleOperationResult(new OperationResult<T?>(
				OperationStatus.NotFound,
				null,
				StatusMessages.Crud<T>.NotFound(id)));
		}

		// Run access validation
		var accessValidationResult = await _accessValidationRunner.Validate(entity, ActionType.Read);
		if (!accessValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<T?>(
				OperationStatus.Unauthorized,
				null,
				StatusMessages.Crud<T>.NoPermission()));
		}

		await _afterActionRunner.Run(entity);

		return _notifier.HandleOperationResult(new OperationResult<T?>(result: entity));
	}
}