#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity;

/// <exclude />
public abstract class UserDataLoader
{
	private readonly IRestClient _client;
	private readonly IOperationResultNotifier _notifier;
	private readonly IUserClaimsFactory<ViewUserDto> _claimsFactory;
	private readonly SienarAuthenticationStateProvider _authStateProvider;

	protected UserDataLoader(
		IRestClient client,
		IOperationResultNotifier notifier,
		IUserClaimsFactory<ViewUserDto> claimsFactory,
		SienarAuthenticationStateProvider authStateProvider)
	{
		_client = client;
		_notifier = notifier;
		_claimsFactory = claimsFactory;
		_authStateProvider = authStateProvider;
	}

	protected async Task<OperationResult<AccountDataResult>> LoadUserData()
	{
		var result = await _client.Get<AccountDataResult>("account");

		if (result.Status is not OperationStatus.Success)
		{
			return _notifier.HandleWebResult(result);
		}

		var userResult = result.Result!.Result!;
		var user = new ViewUserDto
		{
			Username = userResult.Username,
			Roles = userResult.Roles
				.Select(r => new RoleDto
				{
					Name = r
				})
				.ToList()
		};

		var userClaims = _claimsFactory.CreateClaims(user);
		_authStateProvider.Login(userClaims);

		return _notifier.HandleWebResult(result);
	}
}
