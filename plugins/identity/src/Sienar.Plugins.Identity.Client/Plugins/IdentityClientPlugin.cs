using Sienar.Identity.Processors;
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
				.AddBeforeStatusActionHook<LoadUserDataProcessor, Startup>(Client);

			s.TryAddScoped<INotifier, DefaultNotifier>();
			s.TryAddScoped<IUserClaimsFactory<ViewUserDto>, ClientUserClaimsFactory>();

			s
				// Account
				.TryAddGeneralProcessor<ClientLoginProcessor, LoginRequest, LoginResult>(Client)
				.TryAddGeneralProcessor<ClientAccountLockoutProcessor, AccountLockoutRequest, AccountLockoutResult>(Client)
				.TryAddStatusProcessor<ClientLogoutProcessor, LogoutRequest>(Client)
				.TryAddStatusProcessor<ClientRegisterProcessor, RegisterRequest>(Client)
				.TryAddStatusProcessor<ClientConfirmAccountProcessor, ConfirmAccountRequest>(Client)
				.TryAddStatusProcessor<ClientInitiateEmailChangeProcessor, InitiateEmailChangeRequest>(Client)
				.TryAddStatusProcessor<ClientPerformEmailChangeProcessor, PerformEmailChangeRequest>(Client)
				.TryAddStatusProcessor<ClientChangePasswordProcessor, ChangePasswordRequest>(Client)
				.TryAddStatusProcessor<ClientForgotPasswordProcessor, ForgotPasswordRequest>(Client)
				.TryAddStatusProcessor<ClientResetPasswordProcessor, ResetPasswordRequest>(Client)
				.TryAddStatusProcessor<ClientDeleteAccountProcessor, DeleteAccountRequest>(Client)
				.TryAddResultProcessor<LoadUserDataProcessor, AccountDataResult>(Client)

				// Users
				.TryAddStatusProcessor<ClientLockUserAccountProcessor, LockUserAccountRequest>(Client)
				.TryAddStatusProcessor<ClientUnlockUserAccountProcessor, UnlockUserAccountRequest>(Client)
				.TryAddStatusProcessor<ClientManuallyConfirmUserAccountProcessor, ManuallyConfirmUserAccountRequest>(Client)
				.TryAddStatusProcessor<ClientAddUsertoRoleProcessor, AddUserToRoleRequest>(Client)
				.TryAddStatusProcessor<ClientRemoveUserFromRoleProcessor, RemoveUserFromRoleRequest>(Client);

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