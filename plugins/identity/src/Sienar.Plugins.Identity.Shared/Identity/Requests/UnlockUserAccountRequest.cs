namespace Sienar.Identity.Requests;

[RestEndpoint("users/lock", HttpMethods.Delete)]
public class UnlockUserAccountRequest : IRequest
{
	public int UserId { get; set; }
}