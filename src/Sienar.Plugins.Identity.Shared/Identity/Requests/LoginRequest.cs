namespace Sienar.Identity.Requests;

[RestEndpoint("account/login", HttpMethods.Post)]
public class LoginRequest : Honeypot, IRequest
{
	[Required]
	[DisplayName("Username or email")]
	public string AccountName { get; set; } = string.Empty;

	[Required]
	[DisplayName("Password")]
	[DataType(DataType.Password)]
	public string Password { get; set; } = string.Empty;

	[DisplayName("Remember me")]
	public bool RememberMe { get; set; }
}