namespace Sienar.Identity;

/// <summary>
/// Loads the user's account data in the UI after logging in
/// </summary>
public class LoadUserDataOnLogin :
	UserDataLoader,
	IAfterGeneralAction<LoginRequest>
{
	/// <summary>
	/// Creates a new instance of <c>LoadUserDataOnLogin</c>
	/// </summary>
	/// <param name="client">The rest client</param>
	/// <param name="notifier">The operation result notifier</param>
	/// <param name="claimsFactory">The user claims factory</param>
	/// <param name="authStateProvider">The auth state provider</param>
	public LoadUserDataOnLogin(
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
	public Task Handle(LoginRequest _) => LoadUserData();
}
