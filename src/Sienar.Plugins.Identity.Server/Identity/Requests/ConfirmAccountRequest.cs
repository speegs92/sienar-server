namespace Sienar.Identity.Requests;

[RestEndpoint("account/confirm", HttpMethods.Post)]
public class ConfirmAccountRequest : IRequest
{
	[Required]
	public int UserId { get; set; }

	[Required]
	public Guid VerificationCode { get; set; }
}