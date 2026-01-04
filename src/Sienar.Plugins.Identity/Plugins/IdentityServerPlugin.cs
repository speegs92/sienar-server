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
