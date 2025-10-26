namespace Sienar.Infrastructure;

public class DefaultNotifier : INotifier
{
	/// <inheritdoc />
	public void Success(string message)
	{
		
	}

	/// <inheritdoc />
	public void Warning(string message)
	{
		
	}

	/// <inheritdoc />
	public void Info(string message)
	{
		
	}

	/// <inheritdoc />
	public void Error(string message)
	{
		
	}

	/// <inheritdoc />
	public void Notify(Notification notification)
	{
		var isDanger = notification.Type is
			NotificationType.Error or NotificationType.Warning;
	}
}