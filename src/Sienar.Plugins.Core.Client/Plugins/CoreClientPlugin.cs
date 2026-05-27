using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Configuration;
using Sienar.Data;
using Sienar.Extensions;
using Sienar.Infrastructure;
using Sienar.Security;
using static Sienar.Infrastructure.ApplicationType;

namespace Sienar.Plugins;

/// <summary>
/// configures the Sienar app to run with Blazor WASM application support
/// </summary>
[AppConfigurer(typeof(SienarAppConfigurer))]
public class CoreClientPlugin : IPlugin
{
	private readonly IApplicationAdapter _adapter;
	private readonly RoleProvider _roleProvider;
	private readonly IServiceProvider _sp;

	/// <summary>
	/// Creates a new instance of <c>CoreClientPlugin</c>
	/// </summary>
	/// <param name="adapter">The application adapter</param>
	/// <param name="roleProvider">The role provider</param>
	/// <param name="sp">The startup services</param>
	public CoreClientPlugin(
		IApplicationAdapter adapter,
		RoleProvider roleProvider,
		IServiceProvider sp)
	{
		_adapter = adapter;
		_roleProvider = roleProvider;
		_sp = sp;
	}

	/// <inheritdoc />
	public void Configure()
	{
		_roleProvider.Add(Roles.Admin);

		_adapter.AddServices(services =>
		{
			services
				.AddSingleton(_sp.GetRequiredService<GlobalComponentProvider>())
				.AddSingleton(_sp.GetRequiredService<ComponentProvider>())
				.AddSingleton(_sp.GetRequiredService<RoutableAssemblyProvider>())
				.AddSingleton(_sp.GetRequiredService<LayoutProvider>());

			if (_adapter.ApplicationType is not Client)
			{
				return;
			}

			services.TryAddScoped<IUserAccessor, BlazorUserAccessor>();

			services
				.AddAuthorizationCore()
				.AddSienarBlazorUtilities()
				.AddCookieRestClient()
				.AddRestfulEntities()
				.AddBeforeStatusActionHook<AddCsrfTokenToHttpRequestHook, RestClientRequest<CookieRestClient>>(Client)
				.AddBeforeStatusActionHook<RefreshCsrfTokenOnStartup, Startup>(Client)
				.AddScoped(
					typeof(IGeneralProcessor<,>),
					typeof(DefaultClientGeneralProcessor<,>),
					Client)
				.AddScoped(
					typeof(IStatusProcessor<>),
					typeof(DefaultClientStatusProcessor<>),
					Client);
		});
	}

	private class SienarAppConfigurer : IConfigurer<SienarAppBuilder>
	{
		public void Configure(SienarAppBuilder builder)
		{
			builder.AddStartupServices(sp =>
			{
				sp.TryAddSingleton<GlobalComponentProvider>();
				sp.TryAddSingleton<ComponentProvider>();
				sp.TryAddSingleton<RoutableAssemblyProvider>();
				sp.TryAddSingleton<LayoutProvider>();
			});
		}
	}
}
