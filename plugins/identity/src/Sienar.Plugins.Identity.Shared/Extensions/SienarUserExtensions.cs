using System;
using Sienar.Configuration;
using Sienar.Identity;

namespace Sienar.Extensions;

public static class SienarUserExtensions
{
	public static bool ShouldSendEmailConfirmationEmail(
		LoginOptions loginOptions,
		SienarOptions sienarOptions)
		=> loginOptions.RequireConfirmedAccount && sienarOptions.EnableEmail;

	/// <summary>
	/// Determines whether the given user is locked out
	/// </summary>
	/// <param name="user">The user</param>
	/// <returns>Whether the user is locked out</returns>
	public static bool IsLockedOut(this SienarUser user)
		=> IsLockedOutCore(user.LockoutEnd);

	/// <summary>
	/// Determines whether the given user is locked out
	/// </summary>
	/// <param name="user">The user</param>
	/// <returns>Whether the user is locked out</returns>
	public static bool IsLockedOut(this ViewUserDto user)
		=> IsLockedOutCore(user.LockoutEnd);

	private static bool IsLockedOutCore(DateTime? lockoutEnd)
		=> lockoutEnd.HasValue && lockoutEnd.Value > DateTime.UtcNow;
}