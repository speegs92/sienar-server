namespace Sienar.Identity;

public interface IVerificationCodeManager<T>
	where T : class, ISienarIdentityUser<T>
{
	Task<VerificationCode> CreateCode(
		T user,
		string type);

	Task DeleteCode(
		T user,
		string type);

	Task<VerificationCode?> GetCode(
		T user,
		string type);

	VerificationCodeStatus GetCodeStatus(
		VerificationCode? code,
		Guid suppliedCode);

	Task<VerificationCodeStatus> GetCodeStatus(
		T user,
		string type,
		Guid suppliedCode);

	Task<VerificationCodeStatus> VerifyCode(
		T user,
		string type,
		Guid suppliedCode,
		bool deleteIfValid);
}