#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Sienar.Identity.Processors;
using static Sienar.Infrastructure.ApplicationType;

namespace Sienar.Plugins;

/// <exclude />
[AppConfigurer(typeof(IdentityServerAppConfigurer))]
public class IdentityServerPlugin : IPlugin
{
	private readonly WebApplicationBuilder _builder;
	private readonly PluginDataProvider _pluginDataProvider;

	public IdentityServerPlugin(
		WebApplicationBuilder builder,
		PluginDataProvider pluginDataProvider)
	{
		_builder = builder;
		_pluginDataProvider = pluginDataProvider;
	}

	public void Configure()
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

		var services = _builder.Services;
		var config = _builder.Configuration;

		services.AddHttpContextAccessor();

		services.TryAddScoped<IPasswordHasher<SienarUser>, PasswordHasher<SienarUser>>();
		services.TryAddScoped<IPasswordManager, PasswordManager>();
		services.TryAddScoped<IUserClaimsFactory<SienarUser>, ServerUserClaimsFactory>();
		services.TryAddScoped<IUserClaimsPrincipalFactory<SienarUser>, UserClaimsPrincipalFactory>();
		services.TryAddScoped<IVerificationCodeManager, VerificationCodeManager>();

		services.TryAddScoped<IEmailSender, DefaultEmailSender>();


		/************
		 * Identity *
		 ***********/

		services.TryAddScoped<IUserAccessor, HttpContextUserAccessor>();
		services.TryAddScoped<IAccountEmailMessageFactory, AccountEmailMessageFactory>();
		services.TryAddScoped<IAccountEmailManager, AccountEmailManager>();
		services.TryAddScoped<IAccountUrlProvider, AccountUrlProvider>();

		// CRUD
		services
			.AddEfEntity<ViewUserDto, ViewUserMapper, UpsertUserDto, UpsertUserMapper, UpsertUserDto, UpsertUserMapper, SienarUser, SienarUserFilterProcessor>(Server)
			.AddAccessValidator<UserIsAdminAccessValidator<SienarUser>, SienarUser>(Server)
			.AddBeforeDeleteActionHook<RemoveUserRelatedEntitiesHook, SienarUser>()
			.AddStateValidator<EnsureAccountInfoUniqueValidator, SienarUser>(Server)
			.AddEfEntity<LockoutReasonDto, LockoutReasonToEntityMapper, LockoutReasonToDtoMapper, LockoutReason, LockoutReasonFilterProcessor>(Server)
			.AddEfEntity<RoleDto, RoleToEntityMapper, RoleToDtoMapper, SienarRole, SienarRoleFilterProcessor>(Server)

		// Security
			.AddProcessor<LoginProcessor, LoginRequest, LoginResult>()
			.AddStatusProcessor<LogoutProcessor, LogoutRequest>()
			.AddResultProcessor<PersonalDataProcessor, PersonalDataResult>()
			.AddStatusProcessor<UserRoleChangeProcessor, AddUserToRoleRequest>()
			.AddAccessValidator<UserIsAdminAccessValidator<AddUserToRoleRequest>, AddUserToRoleRequest>(Server)
			.AddStatusProcessor<UserRoleChangeProcessor, RemoveUserFromRoleRequest>()
			.AddAccessValidator<UserIsAdminAccessValidator<RemoveUserFromRoleRequest>, RemoveUserFromRoleRequest>(Server)
			.AddStatusProcessor<LockUserAccountProcessor, LockUserAccountRequest>()
			.AddAccessValidator<UserIsAdminAccessValidator<LockUserAccountRequest>, LockUserAccountRequest>(Server)
			.AddStatusProcessor<UnlockUserAccountProcessor, UnlockUserAccountRequest>()
			.AddAccessValidator<UserIsAdminAccessValidator<UnlockUserAccountRequest>, UnlockUserAccountRequest>(Server)
			.AddStatusProcessor<ManuallyConfirmUserAccountProcessor, ManuallyConfirmUserAccountRequest>()
			.AddAccessValidator<UserIsAdminAccessValidator<ManuallyConfirmUserAccountRequest>, ManuallyConfirmUserAccountRequest>(Server)
			.AddStatusProcessor<ChangePasswordProcessor, ChangePasswordRequest>()
			.AddStatusProcessor<ForgotPasswordProcessor, ForgotPasswordRequest>()
			.AddStatusProcessor<ResetPasswordProcessor, ResetPasswordRequest>()
			.AddResultProcessor<GetAccountDataProcessor, AccountDataResult>()
			.AddProcessor<GetLockoutReasonsProcessor, AccountLockoutRequest, AccountLockoutResult>()

		// Registration
			.AddStateValidator<RegistrationOpenValidator, RegisterRequest>(Server)
			.AddStateValidator<AcceptTosValidator, RegisterRequest>(Server)
			.AddStateValidator<EnsureAccountInfoUniqueValidator, RegisterRequest>(Server)
			.AddStatusProcessor<RegisterProcessor, RegisterRequest>()

		// Email
			.AddStatusProcessor<ConfirmAccountProcessor, ConfirmAccountRequest>()
			.AddStatusProcessor<InitiateEmailChangeProcessor, InitiateEmailChangeRequest>()
			.AddStatusProcessor<PerformEmailChangeProcessor, PerformEmailChangeRequest>()

		// Personal data
			.AddBeforeStatusActionHook<RemoveUserRelatedEntitiesHook, DeleteAccountRequest>()
			.AddStatusProcessor<DeleteAccountProcessor, DeleteAccountRequest>();


		/********
		 * Auth *
		 *******/

		services.TryAddScoped<ISignInManager, CookieSignInManager>();


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
