namespace Sienar.Identity.Hooks;

public interface IUserPersonalDataRetriever
{
	/// <summary>
	/// Retrieves personal data 
	/// </summary>
	/// <param name="user"></param>
	/// <returns></returns>
	Task<Dictionary<string, string>> GetUserData(SienarUser user);
}