namespace Sienar.Identity.Requests;

[RestEndpoint("users/roles", HttpMethods.Delete)]
public class RemoveUserFromRoleRequest : IRequest
{
	public int UserId { get; set; }
	public int RoleId { get; set; }
}