namespace Sienar.Identity;

/// <summary>
/// Loads the user's account data in the UI on app startup
/// </summary>
public class LoadUserDataOnStartup :
	UserDataLoader,
	IBeforeStatusAction<Startup>
{
	/// <summary>
	/// Creates a new instance of <c>LoadUserDataOnStartup</c>
	/// </summary>
	/// <param name="client">The rest client</param>
	/// <param name="notifier">The operation result notifier</param>
	/// <param name="claimsFactory">The user claims factory</param>
	/// <param name="authStateProvider">The auth state provider</param>
	public LoadUserDataOnStartup(
		IRestClient client,
		IOperationResultNotifier notifier,
		IUserClaimsFactory<ViewUserDto> claimsFactory,
		SienarAuthenticationStateProvider authStateProvider)
		: base(
			client,
			notifier,
			claimsFactory,
			authStateProvider) {}

	/// <inheritdoc />
	public Task Handle(Startup _) => LoadUserData();
}
