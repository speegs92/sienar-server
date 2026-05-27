namespace Sienar.Infrastructure;

/// <inheritdoc />
public class DefaultOperationResultNotifier : IOperationResultNotifier
{
	private readonly INotifier _notifier;

	/// <summary>
	/// Creates a new instance of <c>DefaultOperationResultNotifier</c>
	/// </summary>
	/// <param name="notifier">The notifier</param>
	public DefaultOperationResultNotifier(INotifier notifier)
	{
		_notifier = notifier;
	}

	/// <inheritdoc />
	public OperationResult<T> HandleWebResult<T>(OperationResult<WebResult<T>> webResult)
	{
		var innerResult = webResult.Result is null ? default(T) : webResult.Result.Result;
		var notifications = webResult.Result?.Notifications;
		var hasNotifications = notifications?.Length > 0;

		if (hasNotifications)
		{
			foreach (var n in notifications!)
			{
				_notifier.Notify(n);
			}
		}

		var operationResult = new OperationResult<T>(
			webResult.Status,
			innerResult,
			hasNotifications ? null : webResult.Message);

		return HandleOperationResult(operationResult);
	}

	/// <inheritdoc />
	public OperationResult<T> HandleOperationResult<T>(OperationResult<T> operationResult)
	{
		if (operationResult.Message is null)
		{
			return operationResult;
		}

		if (operationResult.Status is OperationStatus.Success)
		{
			_notifier.Success(operationResult.Message);
		}
		else
		{
			_notifier.Error(operationResult.Message);
		}

		return operationResult;
	}
}
