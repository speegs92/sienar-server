#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Sienar.Identity.Processors;
using static Sienar.Infrastructure.ApplicationType;

namespace Sienar.Plugins;

/// <exclude />
[AppConfigurer(typeof(IdentityServerAppConfigurer))]
public class IdentityServerPlugin<TUser> : IPlugin
	where TUser : class, ISienarIdentityUser<TUser>, new()
{
	private readonly WebApplicationBuilder _builder;
	private readonly IConfiguration _configuration;
	private readonly ComponentProvider _componentProvider;
	private readonly GlobalComponentProvider _globalComponentProvider;
	private readonly MenuProvider _menuProvider;
	private readonly PluginDataProvider _pluginDataProvider;
	private readonly RoutableAssemblyProvider _routableAssemblyProvider;
	private readonly StyleProvider _styleProvider;

	public IdentityServerPlugin(
		WebApplicationBuilder builder,
		IConfiguration configuration,
		ComponentProvider componentProvider,
		GlobalComponentProvider globalComponentProvider,
		MenuProvider menuProvider,
		PluginDataProvider pluginDataProvider,
		RoutableAssemblyProvider routableAssemblyProvider,
		StyleProvider styleProvider)
	{
		_builder = builder;
		_configuration = configuration;
		_componentProvider = componentProvider;
		_globalComponentProvider = globalComponentProvider;
		_menuProvider = menuProvider;
		_pluginDataProvider = pluginDataProvider;
		_routableAssemblyProvider = routableAssemblyProvider;
		_styleProvider = styleProvider;
	}

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
			Name = "Sienar Core - REST API",
			Description = "Configures Sienar as a collection of REST API endpoints that can be used as a backend for desktop applications, mobile apps, or JavaScript/WebAssembly SPAs.",
			Author = "Christian LeVesque",
			AuthorUrl = "https://levesque.dev",
			Homepage = "https://sienar.io",
			Version = Version.Parse("0.1.1")
		});
	}

	private void SetupRoutableAssemblies()
	{
		_routableAssemblyProvider.Add(typeof(IdentityServerPlugin<TUser>).Assembly);
	}

	private void SetupStyles()
	{
		_styleProvider.Add("/_content/Sienar.Ui/sienar.css");
		_styleProvider.Add("/_content/Sienar.Ui/Sienar.Ui.bundle.scp.css");
	}

	private void SetupServices()
	{
		var services = _builder.Services;
		var config = _builder.Configuration;

		services.AddHttpContextAccessor();

		services.TryAddScoped<IPasswordHasher<TUser>, PasswordHasher<TUser>>();
		services.TryAddScoped<IPasswordManager<TUser>, PasswordManager<TUser>>();
		services.TryAddScoped<IUserClaimsFactory<TUser>, ServerUserClaimsFactory<TUser>>();
		services.TryAddScoped<IUserClaimsPrincipalFactory<TUser>, Identity.UserClaimsPrincipalFactory<TUser>>();
		services.TryAddScoped<IVerificationCodeManager<TUser>, VerificationCodeManager<TUser>>();

		services.TryAddScoped<IEmailSender, DefaultEmailSender>();


		/************
		 * Identity *
		 ***********/

		services.TryAddScoped<IUserAccessor, HttpContextUserAccessor>();
		services.TryAddScoped<IAccountEmailMessageFactory, AccountEmailMessageFactory>();
		services.TryAddScoped<IAccountEmailManager<TUser>, AccountEmailManager<TUser>>();
		services.TryAddScoped<IAccountUrlProvider, AccountUrlProvider>();

		// CRUD
		services
			.AddEfEntity<ViewUserDto, ViewUserMapper<TUser>, UpsertUserDto, UpsertUserMapper<TUser>, UpsertUserDto, UpsertUserMapper<TUser>, TUser, SienarUserFilterProcessor<TUser>>(Server)
			.AddAccessValidator<UserIsAdminAccessValidator<TUser>, TUser>(Server)
			.AddBeforeDeleteActionHook<RemoveIdentityRelationsOnUserDeleted<TUser>, TUser>(Server)
			.AddStateValidator<EnsureUsernameUniqueOnUpsert<TUser>, TUser>(Server)
			.AddStateValidator<EnsureEmailUniqueOnUpsert<TUser>, TUser>(Server)
			.AddEfEntity<LockoutReasonDto, LockoutReasonToEntityMapper<TUser>, LockoutReasonToDtoMapper<TUser>, LockoutReason<TUser>, LockoutReasonFilterProcessor<TUser>>(Server)

		// Security
			.AddGeneralProcessor<LoginProcessor<TUser>, LoginRequest, LoginResult>(Server)
			.AddStatusProcessor<LogoutProcessor<TUser>, LogoutRequest>(Server)
			.AddResultProcessor<PersonalDataProcessor<TUser>, PersonalDataResult>(Server)
			.AddAccessValidator<UserIsAdminAccessValidator<AddUserToRoleRequest>, AddUserToRoleRequest>(Server)
			.AddAccessValidator<UserIsAdminAccessValidator<RemoveUserFromRoleRequest>, RemoveUserFromRoleRequest>(Server)
			.AddStatusProcessor<LockUserAccountProcessor<TUser>, LockUserAccountRequest>(Server)
			.AddAccessValidator<UserIsAdminAccessValidator<LockUserAccountRequest>, LockUserAccountRequest>(Server)
			.AddStatusProcessor<UnlockUserAccountProcessor<TUser>, UnlockUserAccountRequest>(Server)
			.AddAccessValidator<UserIsAdminAccessValidator<UnlockUserAccountRequest>, UnlockUserAccountRequest>(Server)
			.AddStatusProcessor<ManuallyConfirmUserAccountProcessor<TUser>, ManuallyConfirmUserAccountRequest>(Server)
			.AddAccessValidator<UserIsAdminAccessValidator<ManuallyConfirmUserAccountRequest>, ManuallyConfirmUserAccountRequest>(Server)
			.AddStatusProcessor<ChangePasswordProcessor<TUser>, ChangePasswordRequest>(Server)
			.AddStatusProcessor<ForgotPasswordProcessor<TUser>, ForgotPasswordRequest>(Server)
			.AddStatusProcessor<ResetPasswordProcessor<TUser>, ResetPasswordRequest>(Server)
			.AddResultProcessor<GetAccountDataProcessor, AccountDataResult>(Server)
			.AddGeneralProcessor<GetLockoutReasonsProcessor<TUser>, AccountLockoutRequest, AccountLockoutResult>(Server)

		// Registration
			.AddStateValidator<RegistrationOpenValidator, RegisterRequest>(Server)
			.AddStateValidator<AcceptTosValidator, RegisterRequest>(Server)
			.AddStateValidator<EnsureUsernameUniqueOnRegister<TUser>, RegisterRequest>(Server)
			.AddStateValidator<EnsureEmailUniqueOnRegister<TUser>, RegisterRequest>(Server)
			.AddStatusProcessor<RegisterProcessor<TUser>, RegisterRequest>(Server)

		// Email
			.AddStatusProcessor<ConfirmAccountProcessor<TUser>, ConfirmAccountRequest>(Server)
			.AddStatusProcessor<InitiateEmailChangeProcessor<TUser>, InitiateEmailChangeRequest>(Server)
			.AddStatusProcessor<PerformEmailChangeProcessor<TUser>, PerformEmailChangeRequest>(Server)

		// Personal data
			.AddBeforeStatusActionHook<RemoveIdentityRelationsOnOwnAccountDeleted<TUser>, DeleteAccountRequest>(Server)
			.AddStatusProcessor<DeleteAccountProcessor<TUser>, DeleteAccountRequest>(Server);


		/********
		 * Auth *
		 *******/

		services.TryAddScoped<ISignInManager<TUser>, CookieSignInManager<TUser>>();


		/***********
		 * Options *
		 **********/

		services
			.ApplyDefaultConfiguration<SienarOptions>(config.GetSection("Sienar:Core"))
			.ApplyDefaultConfiguration<EmailSenderOptions>(config.GetSection("Sienar:Email:Sender"))
			.ApplyDefaultConfiguration<IdentityEmailSubjectOptions>(config.GetSection("Sienar:Email:IdentityEmailSubjects"))
			.ApplyDefaultConfiguration<LoginOptions>(config.GetSection("Sienar:Login"));
	}
}
