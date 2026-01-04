namespace Sienar.Infrastructure;

/// <summary>
/// A processor which is called on startup by <see cref="SienarAppBuilder"/>
/// </summary>
public class StartupProcessor : IStatusProcessor<Startup>
{
	/// <inheritdoc />
	public Task<OperationResult<bool>> Process(Startup request)
		=> Task.FromResult(new OperationResult<bool>(result: true));
}
