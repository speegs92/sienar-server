using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Security;

namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityReader{TEntity}"/> which reads entities from a REST endpoint
/// </summary>
/// <typeparam name="TDto">The type of the entity to read</typeparam>
public class RestEntityReader<TDto> : IEntityReader<TDto>
	where TDto : EntityBase
{
	private readonly IRestClient _client;
	private readonly ICrudEndpointGenerator<TDto> _endpointGenerator;
	private readonly IOperationResultNotifier _notifier;
	private readonly ILogger<RestEntityReader<TDto>> _logger;
	private readonly IAccessValidationRunner<TDto> _accessValidationRunner;
	private readonly IAfterActionRunner<TDto> _afterActionRunner;

	/// <summary>
	/// Creates a new instance of <c>RestEntityReader</c>
	/// </summary>
	/// <param name="client">The rest client</param>
	/// <param name="endpointGenerator">The endpoint generator for the given DTO</param>
	/// <param name="notifier">The notifier</param>
	/// <param name="logger">The logger</param>
	/// <param name="accessValidationRunner">The access validation runner</param>
	/// <param name="afterActionRunner">The after-action runner</param>
	public RestEntityReader(
		IRestClient client,
		ICrudEndpointGenerator<TDto> endpointGenerator,
		IOperationResultNotifier notifier,
		ILogger<RestEntityReader<TDto>> logger,
		IAccessValidationRunner<TDto> accessValidationRunner,
		IAfterActionRunner<TDto> afterActionRunner)
	{
		_client = client;
		_endpointGenerator = endpointGenerator;
		_notifier = notifier;
		_logger = logger;
		_accessValidationRunner = accessValidationRunner;
		_afterActionRunner = afterActionRunner;
	}

	/// <inheritdoc />
	public async Task<OperationResult<TDto?>> Read(
		int id,
		Filter? filter = null)
	{
		TDto? entity;
		OperationResult<WebResult<TDto?>> result;

		try
		{
			var endpoint = _endpointGenerator.GenerateReadUrl(id);
			result = await _client.Get<TDto>(
				endpoint,
				filter);
			entity = result.Result?.Result;
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return _notifier.HandleOperationResult(new OperationResult<TDto?>(
				OperationStatus.Unknown,
				null,
				StatusMessages.Crud<TDto>.ReadSingleFailed()));
		}

		if (entity is null)
		{
			return _notifier.HandleOperationResult(new OperationResult<TDto?>(
				OperationStatus.NotFound,
				null,
				StatusMessages.Crud<TDto>.NotFound(id)));
		}

		// Run access validation
		var accessValidationResult = await _accessValidationRunner.Validate(entity, ActionType.Read);
		if (!accessValidationResult.Result)
		{
			return _notifier.HandleOperationResult(new OperationResult<TDto?>(
				OperationStatus.Unauthorized,
				null,
				StatusMessages.Crud<TDto>.NoPermission()));
		}

		await _afterActionRunner.Run(entity, ActionType.Read);
		return _notifier.HandleWebResult(result);
	}

	/// <inheritdoc />
	public async Task<OperationResult<PagedQueryResult<TDto>>> Read(Filter? filter = null)
	{
		OperationResult<WebResult<PagedQueryResult<TDto>?>> webResult;
		PagedQueryResult<TDto> queryResult;

		try
		{
			var endpoint = _endpointGenerator.GenerateReadUrl();
			webResult = await _client.Get<PagedQueryResult<TDto>>(
				endpoint,
				filter);
			queryResult = webResult.Result?.Result ?? new PagedQueryResult<TDto>();
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return _notifier.HandleOperationResult(new OperationResult<PagedQueryResult<TDto>>(
				OperationStatus.Unknown,
				new PagedQueryResult<TDto>(),
				StatusMessages.Crud<TDto>.ReadMultipleFailed()));
		}

		foreach (var entity in queryResult.Items)
		{
			await _afterActionRunner.Run(entity, ActionType.ReadAll);
		}

		return _notifier.HandleWebResult(webResult);
	}
}
