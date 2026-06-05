namespace Sienar.Identity.Requests;

[RestEndpoint("account/password", HttpMethods.Delete)]
public class ForgotPasswordRequest : HoneypotDto, IRequest
{
	[Required]
	[DisplayName("Username or email")]
	public string AccountName { get; set; } = string.Empty;
}