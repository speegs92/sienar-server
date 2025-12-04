using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityReadAllActor{T}"/> which reads entities from an EntityFramework <see cref="DbContext"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class EfEntityReadAllActor<T> : IEntityReadAllActor<T>
	where T : EntityBase
{
	private readonly IDbContext _context;
	private readonly IEfFilterProcessor<T> _filterProcessor;
	private readonly ILogger<EfEntityReadAllActor<T>> _logger;
	private readonly IAfterActionRunner<T> _afterActionRunner;
	private readonly IOperationResultNotifier _notifier;

	/// <summary>
	/// Creates a new instance of <c>EfEntityReadActor</c>
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="filterProcessor">The EF filter processor</param>
	/// <param name="logger">The logger</param>
	/// <param name="afterActionRunner">The after-hook action runner</param>
	/// <param name="notifier">The operation result notifier</param>
	public EfEntityReadAllActor(
		IDbContext context,
		IEfFilterProcessor<T> filterProcessor,
		ILogger<EfEntityReadAllActor<T>> logger,
		IAfterActionRunner<T> afterActionRunner,
		IOperationResultNotifier notifier)
	{
		_context = context;
		_filterProcessor = filterProcessor;
		_logger = logger;
		_afterActionRunner = afterActionRunner;
		_notifier = notifier;
	}

	/// <inheritdoc />
	public async Task<OperationResult<PagedQueryResult<T>>> Read(Filter? filter = null)
	{
		PagedQueryResult<T> queryResult;

		try
		{
			filter = _filterProcessor.ModifyFilter(filter, ActionType.ReadAll);
			var entitySet = _context.Set<T>();
			IQueryable<T> entries;
			IQueryable<T> countEntries;

			if (filter is not null)
			{
				entries = ProcessFilter(filter);
				countEntries = _filterProcessor.Search(entitySet, filter);
			}
			else
			{
				entries = entitySet;
				countEntries = entitySet;
			}

			queryResult = new PagedQueryResult<T>(
				await entries.ToListAsync(),
				await countEntries.CountAsync());
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return _notifier.HandleOperationResult(new OperationResult<PagedQueryResult<T>>(
				OperationStatus.Unknown,
				new PagedQueryResult<T>(),
				StatusMessages.Crud<T>.ReadMultipleFailed()));
		}

		foreach (var entity in queryResult.Items)
		{
			await _afterActionRunner.Run(entity, ActionType.ReadAll);
		}

		return _notifier.HandleOperationResult(new OperationResult<PagedQueryResult<T>>(result: queryResult));
	}

	private IQueryable<T> ProcessFilter(
		Filter filter,
		Expression<Func<T, bool>>? predicate = null)
	{
		var result = (IQueryable<T>) _context.Set<T>();
		if (predicate is not null)
		{
			result = result.Where(predicate);
		}

		result = _filterProcessor.Search(result, filter);
		result = _filterProcessor.ProcessIncludes(result, filter);
		var sortPredicate = _filterProcessor.GetSortPredicate(filter.SortName);
		result = filter.SortDescending ?? false
			? result.OrderByDescending(sortPredicate)
			: result.OrderBy(sortPredicate);

		if (filter.Page > 1)
		{
			result = result.Skip((filter.Page - 1) * filter.PageSize);
		}

		// If filter.PageSize == 0, return all results
		if (filter.PageSize > 0)
		{
			result = result.Take(filter.PageSize);
		}

		return result;
	}
}