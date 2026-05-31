namespace Sienar.Identity.Requests;

[RestEndpoint("users/roles", HttpMethods.Post)]
public class AddUserToRoleRequest : IRequest
{
	public int UserId { get; set; }
	public int RoleId { get; set; }
}