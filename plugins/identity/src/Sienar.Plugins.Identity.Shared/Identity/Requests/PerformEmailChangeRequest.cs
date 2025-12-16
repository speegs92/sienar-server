namespace Sienar.Identity.Requests;

[RestEndpoint("account/email", HttpMethods.Patch)]
public class PerformEmailChangeRequest : IRequest
{
	[Required]
	public int UserId { get; set; }

	[Required]
	public Guid VerificationCode { get; set; }
}