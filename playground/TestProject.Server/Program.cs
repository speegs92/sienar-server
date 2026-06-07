using Sienar.Extensions;
using Sienar.Infrastructure;
using TestProject;

await SienarAppBuilder
	.Create(args)
	.AddPlugin<TestProjectPlugin>()
	.Build()
	.RunAsync();
