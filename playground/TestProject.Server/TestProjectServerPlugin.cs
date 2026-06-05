using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Configuration;
using Sienar.Extensions;
using Sienar.Html;
using Sienar.Infrastructure;
using Sienar.Layouts;
using Sienar.Menus;
using Sienar.Plugins;
using TestProject.Client.Extensions;
using TestProject.Data;
using TestProject.UI;

namespace TestProject;

[AppConfigurer(typeof(SienarAppConfigurer))]
public class TestProjectServerPlugin : IPlugin
{
	private readonly WebApplicationBuilder _builder;
	private readonly RoutableAssemblyProvider _routableAssemblyProvider;
	private readonly ComponentProvider _componentProvider;
	private readonly GlobalComponentProvider _globalComponentProvider;
	private readonly MenuProvider _menuProvider;
	private readonly StyleProvider _styleProvider;

	public TestProjectServerPlugin(
		WebApplicationBuilder builder,
		RoutableAssemblyProvider routableAssemblyProvider,
		ComponentProvider componentProvider,
		GlobalComponentProvider globalComponentProvider,
		MenuProvider menuProvider,
		StyleProvider styleProvider)
	{
		_builder = builder;
		_routableAssemblyProvider = routableAssemblyProvider;
		_componentProvider = componentProvider;
		_globalComponentProvider = globalComponentProvider;
		_menuProvider = menuProvider;
		_styleProvider = styleProvider;
	}

	public void Configure()
	{
		_builder.Services
			.AddDbContextForSienar<AppDbContext>(o => o.UseSienarDb())
			.AddDefaultTheme();

		_routableAssemblyProvider.Add(typeof(TestProjectServerPlugin).Assembly);
		_menuProvider.AddMenu();

		ConfigureStyles();
		ConfigureComponents();
	}

	private void ConfigureComponents()
	{
		_globalComponentProvider.DefaultLayout = typeof(DashboardLayout);
		var mainAppComponents = _componentProvider.Access(typeof(DashboardLayout));
		mainAppComponents[DashboardLayoutSections.AppbarLeft] = typeof(Branding);
	}

	private void ConfigureStyles()
	{
		_styleProvider.Add("/styles.css");
		_styleProvider.Add("/TestProject.Client.Wasm.styles.css");
	}

	private class SienarAppConfigurer : IConfigurer<SienarAppBuilder>
	{
		public void Configure(SienarAppBuilder builder)
		{
			builder.AddPlugin<IdentityServerPlugin<AppUser>>();
		}
	}
}
