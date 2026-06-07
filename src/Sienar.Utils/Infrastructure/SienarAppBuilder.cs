using System.Reflection;
using Microsoft.AspNetCore.Builder;

namespace Sienar.Infrastructure;

/// <summary>
/// The Sienar app builder, which is used to create Sienar applications
/// </summary>
public sealed class SienarAppBuilder
{
	private readonly List<Type> _plugins = [];
	private readonly IServiceCollection _startupServices;
	private readonly WebApplicationBuilder _builder;

	/// <summary>
	/// Creates a new <see cref="SienarAppBuilder"/> and registers core Sienar services on its startup service collection
	/// </summary>
	/// <param name="args">The runtime arguments supplied to <c>Program.Main()</c></param>
	private SienarAppBuilder(string[] args)
	{
		_startupServices = new ServiceCollection();
		_builder = WebApplication.CreateBuilder(args);

		_startupServices
			.AddSingleton(_builder)
			.AddSingleton(_builder.Environment)
			.AddSingleton<IConfiguration>(_builder.Configuration)
			.AddSingleton<MenuProvider>()
			.AddSingleton<PluginDataProvider>()
			.AddSingleton<ScriptProvider>()
			.AddSingleton<StyleProvider>()
			.AddSingleton<RoleProvider>()
			.AddSingleton<MiddlewareProvider>()
			.AddSingleton<IStatusActor<Startup>, StartupStatusActor>()
			.AddLogging();
	}

	/// <summary>
	/// Creates a new <c>SienarAppBuilder</c>
	/// </summary>
	/// <param name="args">The runtime arguments supplied to <c>Program.Main()</c></param>
	/// <returns>The Sienar app builder</returns>
	public static SienarAppBuilder Create(string[] args)
	{
		return new SienarAppBuilder(args);
	}

	/// <summary>
	/// Adds an <see cref="IPlugin"/> to the Sienar app
	/// </summary>
	/// <typeparam name="TPlugin">The type of the plugin to add</typeparam>
	/// <returns>The Sienar app builder</returns>
	public SienarAppBuilder AddPlugin<TPlugin>()
		where TPlugin : class, IPlugin
	{
		var type = typeof(TPlugin);

		if (!_plugins.Contains(type))
		{
			var startupConfigurers = type.GetCustomAttributes<AppConfigurerAttribute>();

			// Call startup configurers, if defined
			foreach (var c in startupConfigurers)
			{
				var configurer = (IConfigurer<SienarAppBuilder>)Activator.CreateInstance(c.Configurer)!;
				configurer.Configure(this);
			}

			// Register plugin
			_plugins.Add(type);
			_startupServices.AddSingleton(type);
		}

		return this;
	}

	/// <summary>
	/// Adds services to the startup DI container
	/// </summary>
	/// <param name="configurer">The <see cref="Action"/> to call against the app's startup <see cref="IServiceCollection"/></param>
	/// <returns>The Sienar app builder</returns>
	public SienarAppBuilder AddStartupServices(Action<IServiceCollection> configurer)
	{
		configurer(_startupServices);
		return this;
	}

	/// <summary>
	/// Builds the final application and returns it
	/// </summary>
	/// <returns>The new application</returns>
	public async Task<WebApplication> Build()
	{
		_builder.Services
			.AddSienarCoreUtilities();

		var container = _startupServices.BuildServiceProvider();
		await using var startupScope = container.CreateAsyncScope();
		var sp = startupScope.ServiceProvider;

		foreach (var pluginType in _plugins)
		{
			var plugin = (IPlugin)sp.GetRequiredService(pluginType);
			plugin.Configure();
		}

		var actor = sp.GetRequiredService<IStatusActor<Startup>>();
		await actor.Execute(new Startup());

		_builder.Services
			.AddSingleton(sp.GetRequiredService<MenuProvider>())
			.AddSingleton(sp.GetRequiredService<PluginDataProvider>())
			.AddSingleton(sp.GetRequiredService<ScriptProvider>())
			.AddSingleton(sp.GetRequiredService<StyleProvider>())
			.AddSingleton(sp.GetRequiredService<RoleProvider>())
			.AddSingleton<LayoutProvider>();

		var app = _builder.Build();

		var middlewareProvider = sp.GetRequiredService<MiddlewareProvider>();

		foreach (var middleware in middlewareProvider.AggregatePrioritized())
		{
			middleware(app);
		}

		return app;
	}
}
