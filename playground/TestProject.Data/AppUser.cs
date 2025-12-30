using System;
using System.Collections.Generic;
using Sienar.Identity;

namespace TestProject.Data;

/// <summary>
/// The test project's user entity
/// </summary>
public class AppUser : ISienarIdentityUser<AppUser>
{
	/// <inheritdoc />
	public int Id { get; set; }

	/// <inheritdoc />
	public Guid ConcurrencyStamp { get; set; }

	/// <inheritdoc />
	public string Username { get; set; }

	/// <inheritdoc />
	public string NormalizedUsername { get; set; }

	/// <inheritdoc />
	public string Email { get; set; }

	/// <inheritdoc />
	public string NormalizedEmail { get; set; }

	/// <inheritdoc />
	public List<string> Roles { get; set; }

	/// <inheritdoc />
	public string PasswordHash { get; set; }

	/// <inheritdoc />
	public int LoginFailedCount { get; set; }

	/// <inheritdoc />
	public DateTime? LockoutEnd { get; set; }

	/// <inheritdoc />
	public List<VerificationCode> VerificationCodes { get; set; }

	/// <inheritdoc />
	public List<LockoutReason<AppUser>> LockoutReasons { get; set; }

	/// <inheritdoc />
	public bool EmailConfirmed { get; set; }

	/// <inheritdoc />
	public string? PendingEmail { get; set; }

	/// <inheritdoc />
	public string? NormalizedPendingEmail { get; set; }
}
