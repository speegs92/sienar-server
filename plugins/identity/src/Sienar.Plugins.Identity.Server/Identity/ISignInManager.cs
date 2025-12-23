namespace Sienar.Identity;

public interface ISignInManager<T>
	where T : class, ISienarIdentityUser<T>
{
	/// <summary>
	/// Signs a new user in to the current session
	/// </summary>
	/// <param name="user">The user to sign in</param>
	/// <param name="isPersistent">Whether the login session should be persistent</param>
	Task SignIn(T user, bool isPersistent);

	/// <summary>
	/// Signs the current user out
	/// </summary>
	Task SignOut();
}