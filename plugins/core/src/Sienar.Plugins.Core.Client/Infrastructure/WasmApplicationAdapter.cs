using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <summary>
/// Maps Sienar application method calls to underlying <see cref="WebAssemblyHostBuilder"/> method calls
/// </summary>
public class WasmApplicationAdapter : IApplicationAdapter<WebAssemblyHostBuilder>
{
	/// <inheritdoc />
	public ApplicationType ApplicationType => ApplicationType.Client;

	/// <inheritdoc />
	public WebAssemblyHostBuilder Builder { get; private set; } = null!;

	/// <inheritdoc />
	public void Create(
		string[] args,
		IServiceCollection startupServices)
	{
		Builder = WebAssemblyHostBuilder.CreateDefault(args);

		startupServices
			.AddSingleton(Builder)
			.AddSingleton(Builder.HostEnvironment)
			.AddSingleton<IConfiguration>(Builder.Configuration)
			.AddSingleton<IApplicationAdapter>(this);
	}

	/// <inheritdoc />
	public async Task<T> Build<T>(IServiceProvider sp)
		where T : class
	{
		var app = Builder.Build();
		await using var scope = app.Services.CreateAsyncScope();

		var startupActor = scope.ServiceProvider.GetRequiredService<IStatusActor<Startup>>();
		await startupActor.Execute(new Startup());

		return (app as T)!;
	}

	/// <inheritdoc />
	public void AddServices(Action<IServiceCollection> configurer)
	{
		configurer(Builder.Services);
	}
}
