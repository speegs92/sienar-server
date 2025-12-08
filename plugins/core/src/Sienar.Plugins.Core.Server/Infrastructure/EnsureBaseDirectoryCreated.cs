using System.IO;

namespace Sienar.Infrastructure;

/// <summary>
/// Ensures that the web application's files directory exists at app startup
/// </summary>
public class EnsureBaseDirectoryCreated : IBeforeStatusAction<Startup>
{
	/// <inheritdoc />
	public Task Handle(Startup request)
	{
		FileUtils.BaseAppFolderName = Path.Combine(
			Environment.CurrentDirectory,
			"../SienarFiles");
		FileUtils.EnsureDirectoryExists(FileUtils.BaseAppFolderName);

		return Task.CompletedTask;
	}
}
