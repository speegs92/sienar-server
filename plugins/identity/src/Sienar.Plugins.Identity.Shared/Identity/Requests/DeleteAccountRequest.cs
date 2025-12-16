namespace Sienar.Identity.Requests;

[RestEndpoint("account", HttpMethods.Delete)]
public class DeleteAccountRequest : IRequest
{
	[Required]
	[DisplayName("Password")]
	[DataType(DataType.Password)]
	public string Password { get; set; } = default!;
}