namespace Sienar.Identity.Requests;

public class ForgotPasswordRequest : Honeypot, IRequest
{
	[Required]
	[DisplayName("Username or email")]
	public string AccountName { get; set; } = string.Empty;
}