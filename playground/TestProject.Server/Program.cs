using Microsoft.AspNetCore.Builder;
using Sienar.Extensions;
using Sienar.Infrastructure;
using TestProject;

var app = await SienarAppBuilder
	.Create(args)
	.AddWebAdapter()
	.AddPlugin<TestProjectPlugin>()
	.Build<WebApplication>();

await app.RunAsync();
