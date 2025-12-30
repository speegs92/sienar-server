#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Microsoft.AspNetCore.Identity;

namespace Sienar.Identity;

/// <exclude />
public class PasswordManager<T> : IPasswordManager<T>
	where T : class, ISienarIdentityUser<T>
{
	private readonly IPasswordHasher<T> _passwordHasher;
	private readonly ISienarDbContext<T> _context;

	public PasswordManager(
		IPasswordHasher<T> passwordHasher,
		ISienarDbContext<T> context)
	{
		_passwordHasher = passwordHasher;
		_context = context;
	}

	/// <inheritdoc />
	public async Task UpdatePassword(
		T user,
		string newPassword)
	{
		user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
		_context.Users.Update(user);
		await _context.SaveChangesAsync();
	}

	/// <inheritdoc />
	public async Task<bool> VerifyPassword(T user, string password)
	{
		var verification = _passwordHasher.VerifyHashedPassword(
			user,
			user.PasswordHash,
			password);

		if (verification == PasswordVerificationResult.Failed)
		{
			return false;
		}

		if (verification == PasswordVerificationResult.SuccessRehashNeeded)
		{
			user.PasswordHash = _passwordHasher.HashPassword(user, password);
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
		}

		return true;
	}
}