#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Sienar.Infrastructure;

/// <exclude />
public class DefaultAuthenticationBuilderConfigurer
	: IConfigurer<AuthenticationBuilder>
{
	public void Configure(AuthenticationBuilder options)
	{
		options.AddCookie(
			CookieAuthenticationDefaults.AuthenticationScheme,
			o =>
			{
				o.LoginPath = null;
				o.AccessDeniedPath = null;
				o.LogoutPath = null;

				o.Events.OnRedirectToLogin = ctx =>
				{
					ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
					return Task.CompletedTask;
				};

				o.Events.OnRedirectToAccessDenied = ctx =>
				{
					ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
					return Task.CompletedTask;
				};
			});
	}
}