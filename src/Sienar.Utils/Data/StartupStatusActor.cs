namespace Sienar.Data;

/// <summary>
/// Processes status actions on startup
/// </summary>
public class StartupStatusActor : IStatusActor<Startup>
{
	private readonly ILogger<StartupStatusActor> _logger;
	private readonly IEnumerable<IStatusProcessor<Startup>> _processors;

	/// <summary>
	/// Creates a new instance of <c>StartupStatusActor</c>
	/// </summary>
	/// <param name="logger">The logger</param>
	/// <param name="processors">The startup processors</param>
	public StartupStatusActor(
		ILogger<StartupStatusActor> logger,
		IEnumerable<IStatusProcessor<Startup>> processors)
	{
		_logger = logger;
		_processors = processors;
	}

	/// <inheritdoc />
	public async Task<OperationResult<bool>> Execute(Startup request)
	{
		foreach (var processor in _processors)
		{
			OperationResult<bool> result;

			try
			{
				result = await processor.Process(request);
			}
			catch (Exception e)
			{
				_logger.LogError(
					e,
					"Startup status action {processorName} failed",
					processor.GetType());

				continue;
			}

			if (result.Status is not OperationStatus.Success &&
				!string.IsNullOrEmpty(result.Message))
			{
				_logger.LogError(
					"Startup status action failed with the following message: {message}",
					result.Message);
			}
		}

		return new OperationResult<bool>(result: true);
	}
}
