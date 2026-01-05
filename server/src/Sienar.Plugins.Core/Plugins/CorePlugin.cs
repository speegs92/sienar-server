namespace Sienar.Plugins;

/// <summary>
/// Configures the Sienar app to run as a web application with auth, CORS, and other core web-based services
/// </summary>
[AppConfigurer(typeof(SienarAppConfigurer))]
public class CorePlugin : IPlugin
{
	private readonly WebApplicationBuilder _builder;

	/// <summary>
	/// Creates a new instance of <c>CoreServerPlugin</c>
	/// </summary>
	/// <param name="builder">The application adapter</param>
	public CorePlugin(WebApplicationBuilder builder)
	{
		_builder = builder;
	}

	/// <inheritdoc />
	public void Configure()
	{
		_builder.Services
			.AddEntityFramework()
			.AddBeforeStatusActionHook<EnsureBaseDirectoryCreated, Startup>(ApplicationType.Server);
	}

	private class SienarAppConfigurer : IConfigurer<SienarAppBuilder>
	{
		public void Configure(SienarAppBuilder builder)
		{
			builder
				.AddPlugin<CoreSecurityPlugin>()
				.AddPlugin<CoreMvcPlugin>()
				.AddStartupServices(sp => sp.AddSingleton<MiddlewareProvider>());
		}
	}
}
