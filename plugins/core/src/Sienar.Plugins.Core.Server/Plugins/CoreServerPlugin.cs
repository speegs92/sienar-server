namespace Sienar.Plugins;

/// <summary>
/// Configures the Sienar app to run as a web application with auth, CORS, and other core web-based services
/// </summary>
[AppConfigurer(typeof(SienarAppConfigurer))]
public class CoreServerPlugin : IPlugin
{
	private readonly IApplicationAdapter _adapter;

	/// <summary>
	/// Creates a new instance of <c>CoreServerPlugin</c>
	/// </summary>
	/// <param name="adapter">The application adapter</param>
	public CoreServerPlugin(IApplicationAdapter adapter)
	{
		_adapter = adapter;
	}

	/// <inheritdoc />
	public void Configure()
	{
		_adapter.AddServices(sp =>
		{
			sp
				.AddEntityFramework()
				.AddBeforeStatusActionHook<EnsureBaseDirectoryCreated, Startup>();
		});
	}

	private class SienarAppConfigurer : IConfigurer<SienarAppBuilder>
	{
		public void Configure(SienarAppBuilder builder)
		{
			builder
				.AddPlugin<CoreSecurityPlugin>()
				.AddPlugin<CoreMvcPlugin>()
				.AddPlugin<CoreBlazorPlugin>()
				.AddStartupServices(sp => sp.AddSingleton<MiddlewareProvider>());
		}
	}
}
