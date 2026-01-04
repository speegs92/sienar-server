namespace Sienar.Identity;

/// <summary>
/// The base user for all Sienar applications
/// </summary>
public interface IUser : IEntity
{
	/// <summary>
	/// The user's username
	/// </summary>
	string Username { get; set; }

	/// <summary>
	/// The user's normalized username
	/// </summary>
	string NormalizedUsername { get; set; }

	/// <summary>
	/// The user's email address
	/// </summary>
	string Email { get; set; }

	/// <summary>
	/// The user's normalized email addres
	/// </summary>
	string NormalizedEmail { get; set; }

	/// <summary>
	/// The user's roles
	/// </summary>
	List<string> Roles { get; set; }
}