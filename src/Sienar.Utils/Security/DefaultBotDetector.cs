#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Security;

/// <exclude />
public class DefaultBotDetector : IBotDetector
{
	private readonly ILogger<DefaultBotDetector> _logger;

	public DefaultBotDetector(ILogger<DefaultBotDetector> logger)
	{
		_logger = logger;
	}

	public bool IsSpambot(HoneypotDto honeypot)
	{
		_logger.LogInformation(
			"Form submission completed in {time} seconds",
			honeypot.TimeToComplete.TotalSeconds);

		var isSpambot = !string.IsNullOrEmpty(honeypot.SecretKeyField);

		if (isSpambot)
		{
			_logger.LogError(
				"Spambot detected! Value passed to honeypot was '{value}'",
				honeypot.SecretKeyField);
		}

		return isSpambot;
	}
}
