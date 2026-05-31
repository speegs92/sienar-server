namespace Sienar.Infrastructure;

/// <summary>
/// Maps Sienar application method calls to underlying <see cref="WebApplicationBuilder"/> method calls
/// </summary>
public class WebApplicationAdapter : IApplicationAdapter<WebApplicationBuilder>
{
	/// <inheritdoc />
	public ApplicationType ApplicationType => ApplicationType.Server;

	/// <inheritdoc />
	public WebApplicationBuilder Builder { get; private set; } = null!;

	/// <inheritdoc />
	public void Create(
		string[] args,
		IServiceCollection startupServices)
	{
		Builder = WebApplication.CreateBuilder(args);

		startupServices
			.AddSingleton(Builder)
			.AddSingleton(Builder.Environment)
			.AddSingleton<IConfiguration>(Builder.Configuration)
			.AddSingleton<IApplicationAdapter>(this);
	}

	/// <inheritdoc />
	public async Task<T> Build<T>(IServiceProvider sp)
		where T : class
	{
		var app = Builder.Build();

		var middlewareProvider = sp.GetRequiredService<MiddlewareProvider>();

		foreach (var middleware in middlewareProvider.AggregatePrioritized())
		{
			middleware(app);
		}

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
