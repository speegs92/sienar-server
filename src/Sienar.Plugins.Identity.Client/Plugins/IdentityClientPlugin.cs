using Sienar.Layouts;
using Sienar.Ui.Views;
using static Sienar.Infrastructure.ApplicationType;

namespace Sienar.Plugins;

/// <summary>
/// Configures the Sienar application to run the WASM client
/// </summary>
[AppConfigurer(typeof(SienarAppConfigurer))]
public class IdentityClientPlugin : IPlugin
{
	private readonly IApplicationAdapter _adapter;
	private readonly IConfiguration _configuration;
	private readonly ComponentProvider _componentProvider;
	private readonly GlobalComponentProvider _globalComponentProvider;
	private readonly MenuProvider _menuProvider;
	private readonly PluginDataProvider _pluginDataProvider;
	private readonly RoutableAssemblyProvider _routableAssemblyProvider;
	private readonly StyleProvider _styleProvider;

	/// <summary>
	/// Creates a new instance of <c>CoreClientPlugin</c>
	/// </summary>
	public IdentityClientPlugin(
		IApplicationAdapter adapter,
		IConfiguration configuration,
		ComponentProvider componentProvider,
		GlobalComponentProvider globalComponentProvider,
		MenuProvider menuProvider,
		PluginDataProvider pluginDataProvider,
		RoutableAssemblyProvider routableAssemblyProvider,
		StyleProvider styleProvider)
	{
		_adapter = adapter;
		_configuration = configuration;
		_componentProvider = componentProvider;
		_globalComponentProvider = globalComponentProvider;
		_menuProvider = menuProvider;
		_pluginDataProvider = pluginDataProvider;
		_routableAssemblyProvider = routableAssemblyProvider;
		_styleProvider = styleProvider;
	}

	/// <inheritdoc />
	public void Configure()
	{
		SetupComponents();
		SetupMenu();
		SetupPluginData();
		SetupRoutableAssemblies();
		SetupStyles();
		SetupServices();
	}

	private void SetupComponents()
	{
		_componentProvider
			.Access(typeof(DashboardLayout))
			.TryAddComponent<DrawerHeader>(DashboardLayoutSections.SidebarHeader)
			.TryAddComponent<DrawerFooter>(DashboardLayoutSections.SidebarFooter);

		_globalComponentProvider.DefaultLayout ??= typeof(DashboardLayout);
		_globalComponentProvider.NotFoundView ??= typeof(NotFound);
		_globalComponentProvider.UnauthorizedView ??= typeof(Unauthorized);
		_globalComponentProvider.DefaultMenus = [IdentityMenus.Main, IdentityMenus.Info];
	}

	private void SetupMenu()
	{
		_menuProvider
			.CreateMainMenu()
			.CreateUserSettingsMenu()
			.CreateInfoMenu()
			.CreateUserManagementMenu();
	}

	private void SetupPluginData()
	{
		_pluginDataProvider.Add(new PluginData
		{
			Name = "Sienar Core Client",
			Version = Version.Parse("0.1.0"),
			Author = "Christian LeVesque",
			AuthorUrl = "https://levesque.dev",
			Description = "The Sienar Core Client plugin provides all of the main services and configuration required to render the Sienar Core user interface.",
			Homepage = "https://sienar.io"
		});
	}

	private void SetupRoutableAssemblies()
	{
		_routableAssemblyProvider.Add(typeof(IdentityClientPlugin).Assembly);
	}

	private void SetupStyles()
	{
		_styleProvider.Add("/_content/Sienar.Ui/sienar.css");
		_styleProvider.Add("/_content/Sienar.Ui/Sienar.Ui.bundle.scp.css");
	}

	private void SetupServices()
	{
		if (_adapter.ApplicationType is not Client) return;

		_adapter.AddServices(s =>
		{
			// Infrastructure
			s
				.AddBeforeStatusActionHook<LoadUserDataOnStartup, Startup>(Client);

			s.TryAddScoped<INotifier, DefaultNotifier>();
			s.TryAddScoped<IUserClaimsFactory<ViewUserDto>, ClientUserClaimsFactory>();

			s
				// Account
				.AddAfterGeneralActionHook<LoadUserDataOnLogin, LoginRequest>(Client)
				.AddAfterGeneralActionHook<RefreshCsrfTokenOnLogin, LoginRequest>(Client)
				.AddAfterStatusActionHook<LogOutUiAfterLogout, LogoutRequest>(Client)
				.AddAfterStatusActionHook<RefreshCsrfTokenOnLogout, LogoutRequest>(Client)
				.AddStateValidator<EnsureTosAccepted, RegisterRequest>(Client)
				.AddAfterStatusActionHook<LogOutAfterDeletingAccount, DeleteAccountRequest>(Client);

			s.ApplyDefaultConfiguration<SienarOptions>(
				_configuration.GetSection("Sienar:Core"));
		});
	}

	private class SienarAppConfigurer : IConfigurer<SienarAppBuilder>
	{
		public void Configure(SienarAppBuilder builder)
		{
			builder
				.AddPlugin<MudBlazorPlugin>()
				.AddPlugin<CoreClientPlugin>();
		}
	}
}