#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Processors;

/// <exclude />
public class LoadUserDataProcessor
	: IResultProcessor<AccountDataResult>, IBeforeTask<SienarStartupActor>
{
	private readonly IRestClient _client;
	private readonly IOperationResultNotifier _notifier;
	private readonly IUserClaimsFactory _claimsFactory;
	private readonly SienarAuthenticationStateProvider _authStateProvider;

	public LoadUserDataProcessor(
		IRestClient client,
		IOperationResultNotifier notifier,
		IUserClaimsFactory claimsFactory,
		SienarAuthenticationStateProvider authStateProvider)
	{
		_client = client;
		_notifier = notifier;
		_claimsFactory = claimsFactory;
		_authStateProvider = authStateProvider;
	}

	Task<OperationResult<AccountDataResult?>> IResultProcessor<AccountDataResult>.Process()
		=> LoadUserData();

	Task IBeforeTask<SienarStartupActor>.Handle(SienarStartupActor? a)
		=> LoadUserData();

	private async Task<OperationResult<AccountDataResult?>> LoadUserData()
	{
		var result = await _client.Get<AccountDataResult>("account");

		if (result.Status is not OperationStatus.Success)
		{
			return _notifier.HandleWebResult(result);
		}

		var userResult = result.Result!.Result!;
		var user = new SienarUser
		{
			Username = userResult.Username,
			Roles = userResult.Roles
				.Select(r => new SienarRole
				{
					Name = r
				})
				.ToList()
		};

		var userClaims = _claimsFactory.CreateClaims(user);
		_authStateProvider.NotifyUserAuthentication(userClaims, true);

		return _notifier.HandleWebResult(result);
	}
}
