namespace Sienar.Identity.Requests;

[RestEndpoint("users/confirm", HttpMethods.Patch)]
public class ManuallyConfirmUserAccountRequest : IRequest
{
	public int UserId { get; set; }
}