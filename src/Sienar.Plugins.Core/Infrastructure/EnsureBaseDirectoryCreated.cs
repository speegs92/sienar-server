using System.IO;

namespace Sienar.Infrastructure;

/// <summary>
/// Ensures that the web application's files directory exists at app startup
/// </summary>
public class EnsureBaseDirectoryCreated : IStatusProcessor<Startup>
{
	/// <inheritdoc />
	public Task<OperationResult<bool>> Process(Startup request)
	{
		FileUtils.BaseAppFolderName = Path.Combine(
			Environment.CurrentDirectory,
			"../SienarFiles");
		FileUtils.EnsureDirectoryExists(FileUtils.BaseAppFolderName);

		return Task.FromResult(new OperationResult<bool>(result: true));
	}
}
