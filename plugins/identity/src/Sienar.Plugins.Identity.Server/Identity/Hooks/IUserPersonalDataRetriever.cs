namespace Sienar.Identity.Hooks;

public interface IUserPersonalDataRetriever<T>
	where T : class, ISienarIdentityUser<T>
{
	/// <summary>
	/// Retrieves personal data 
	/// </summary>
	/// <param name="user"></param>
	/// <returns></returns>
	Task<Dictionary<string, string>> GetUserData(T user);
}