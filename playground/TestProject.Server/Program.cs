using Sienar.Infrastructure;
using TestProject;

var app = await SienarAppBuilder
	.Create(args)
	.AddPlugin<TestProjectPlugin>()
	.Build();

await app.RunAsync();
