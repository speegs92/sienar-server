namespace Sienar.Identity.Requests;

[RestEndpoint("account/login", HttpMethods.Delete)]
public class LogoutRequest : IRequest;