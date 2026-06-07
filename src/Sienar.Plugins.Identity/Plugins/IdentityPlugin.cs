#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Sienar.Identity.Processors;

namespace Sienar.Plugins;

/// <exclude />
[AppConfigurer(typeof(IdentityAppConfigurer))]
public class IdentityPlugin<TUser> : IPlugin
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

	public IdentityPlugin(
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
		_routableAssemblyProvider.Add(typeof(IdentityPlugin<TUser>).Assembly);
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
		services.TryAddScoped<IUserClaimsFactory<TUser>, UserClaimsFactory<TUser>>();
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
			.AddEfEntity<ViewUserDto, ViewUserMapper<TUser>, UpsertUserDto, UpsertUserMapper<TUser>, UpsertUserDto, UpsertUserMapper<TUser>, TUser, SienarUserFilterProcessor<TUser>>()
			.AddAccessValidator<UserIsAdminAccessValidator<TUser>, TUser>()
			.AddBeforeDeleteActionHook<RemoveIdentityRelationsOnUserDeleted<TUser>, TUser>()
			.AddStateValidator<EnsureUsernameUniqueOnUpsert<TUser>, TUser>()
			.AddStateValidator<EnsureEmailUniqueOnUpsert<TUser>, TUser>()
			.AddEfEntity<LockoutReasonDto, LockoutReasonToEntityMapper<TUser>, LockoutReasonToDtoMapper<TUser>, LockoutReason<TUser>, LockoutReasonFilterProcessor<TUser>>()

		// Security
			.AddGeneralProcessor<LoginProcessor<TUser>, LoginRequest, LoginResult>()
			.AddStatusProcessor<LogoutProcessor<TUser>, LogoutRequest>()
			.AddResultProcessor<PersonalDataProcessor<TUser>, PersonalDataResult>()
			.AddAccessValidator<UserIsAdminAccessValidator<AddUserToRoleRequest>, AddUserToRoleRequest>()
			.AddAccessValidator<UserIsAdminAccessValidator<RemoveUserFromRoleRequest>, RemoveUserFromRoleRequest>()
			.AddStatusProcessor<LockUserAccountProcessor<TUser>, LockUserAccountRequest>()
			.AddAccessValidator<UserIsAdminAccessValidator<LockUserAccountRequest>, LockUserAccountRequest>()
			.AddStatusProcessor<UnlockUserAccountProcessor<TUser>, UnlockUserAccountRequest>()
			.AddAccessValidator<UserIsAdminAccessValidator<UnlockUserAccountRequest>, UnlockUserAccountRequest>()
			.AddStatusProcessor<ManuallyConfirmUserAccountProcessor<TUser>, ManuallyConfirmUserAccountRequest>()
			.AddAccessValidator<UserIsAdminAccessValidator<ManuallyConfirmUserAccountRequest>, ManuallyConfirmUserAccountRequest>()
			.AddStatusProcessor<ChangePasswordProcessor<TUser>, ChangePasswordRequest>()
			.AddStatusProcessor<ForgotPasswordProcessor<TUser>, ForgotPasswordRequest>()
			.AddStatusProcessor<ResetPasswordProcessor<TUser>, ResetPasswordRequest>()
			.AddResultProcessor<GetAccountDataProcessor, AccountDataResult>()
			.AddGeneralProcessor<GetLockoutReasonsProcessor<TUser>, AccountLockoutRequest, AccountLockoutResult>()

		// Registration
			.AddStateValidator<RegistrationOpenValidator, RegisterRequest>()
			.AddStateValidator<AcceptTosValidator, RegisterRequest>()
			.AddStateValidator<EnsureUsernameUniqueOnRegister<TUser>, RegisterRequest>()
			.AddStateValidator<EnsureEmailUniqueOnRegister<TUser>, RegisterRequest>()
			.AddStatusProcessor<RegisterProcessor<TUser>, RegisterRequest>()

		// Email
			.AddStatusProcessor<ConfirmAccountProcessor<TUser>, ConfirmAccountRequest>()
			.AddStatusProcessor<InitiateEmailChangeProcessor<TUser>, InitiateEmailChangeRequest>()
			.AddStatusProcessor<PerformEmailChangeProcessor<TUser>, PerformEmailChangeRequest>()

		// Personal data
			.AddBeforeStatusActionHook<RemoveIdentityRelationsOnOwnAccountDeleted<TUser>, DeleteAccountRequest>()
			.AddStatusProcessor<DeleteAccountProcessor<TUser>, DeleteAccountRequest>();


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
