namespace Sienar.Identity;

public class VerificationCodeManager<T> : IVerificationCodeManager<T>
	where T : class, ISienarIdentityUser<T>
{
	private readonly ISienarDbContext<T> _context;

	public VerificationCodeManager(ISienarDbContext<T> context)
	{
		_context = context;
	}

	/// <inheritdoc/>
	public async Task<VerificationCode> CreateCode(
		T user,
		string type)
	{
		await _context.Users
			.Entry(user)
			.Collection(u => u.VerificationCodes)
			.LoadAsync();

		await DeleteCode(user, type);

		var code = new VerificationCode
		{
			Code = Guid.NewGuid(),
			Type = type,
			ExpiresAt = GetVerificationCodeExpiration(type)
		};

		user.VerificationCodes.Add(code);
		_context.Users.Update(user);
		await _context.SaveChangesAsync();

		return code;
	}

	/// <inheritdoc/>
	public Task DeleteCode(
		T user,
		string type)
		=> DeleteVerificationCode(user, type, true);

	private async Task DeleteVerificationCode(
		T user,
		string type,
		bool saveChanges)
	{
		await _context.Users
			.Entry(user)
			.Collection(u => u.VerificationCodes)
			.LoadAsync();

		var code = user.VerificationCodes.FirstOrDefault(c => c.Type == type);
		if (code is null)
		{
			return;
		}

		user.VerificationCodes.Remove(code);
		_context.Remove(code);

		if (saveChanges)
		{
			await _context.SaveChangesAsync();
		}
	}

	/// <inheritdoc/>
	public async Task<VerificationCode?> GetCode(
		T user,
		string type)
	{
		await _context.Users
			.Entry(user)
			.Collection(u => u.VerificationCodes)
			.LoadAsync();

		return user.VerificationCodes.FirstOrDefault(v => v.Type == type);
	}

	public VerificationCodeStatus GetCodeStatus(
		VerificationCode? code,
		Guid suppliedCode)
	{
		if (code is null 
		|| code.Code != suppliedCode)
		{
			return VerificationCodeStatus.Invalid;
		}

		if (code.ExpiresAt < DateTime.UtcNow)
		{
			return VerificationCodeStatus.Expired;
		}

		return VerificationCodeStatus.Valid;
	}

	public async Task<VerificationCodeStatus> GetCodeStatus(
		T user,
		string type,
		Guid suppliedCode)
	{
		var code = await GetCode(user, type);
		return GetCodeStatus(code, suppliedCode);
	}

	public async Task<VerificationCodeStatus> VerifyCode(
		T user,
		string type,
		Guid suppliedCode,
		bool deleteIfValid)
	{
		var code = await GetCode(user, type);
		var status = GetCodeStatus(code, suppliedCode);
		if (status == VerificationCodeStatus.Valid && deleteIfValid)
		{
			await DeleteCode(user, type);
		}

		return status;
	}

	private static DateTime GetVerificationCodeExpiration(string type)
	{
		return DateTime.UtcNow.AddMinutes(30);
	}
}