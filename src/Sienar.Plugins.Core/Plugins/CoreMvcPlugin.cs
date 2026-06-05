using Microsoft.Extensions.Hosting;

namespace Sienar.Plugins;

/// <summary>
/// Configures the Sienar app to run as an MVC application with controllers, views, and Razor Pages
/// </summary>
public class CoreMvcPlugin : IPlugin
{
	private readonly WebApplicationBuilder _builder;
	private readonly MiddlewareProvider _middlewareProvider;
	private readonly IEnumerable<IConfigurer<MvcOptions>> _mvcConfigurers;
	private readonly IEnumerable<IConfigurer<IMvcBuilder>> _additionalMvcConfigurers;

	/// <summary>
	/// Creates a new instance of <c>MvcPlugin</c>
	/// </summary>
	public CoreMvcPlugin(
		WebApplicationBuilder builder,
		MiddlewareProvider middlewareProvider, 
		IEnumerable<IConfigurer<MvcOptions>> mvcConfigurers,
		IEnumerable<IConfigurer<IMvcBuilder>> additionalMvcConfigurers)
	{
		_builder = builder;
		_middlewareProvider = middlewareProvider;
		_mvcConfigurers = mvcConfigurers;
		_additionalMvcConfigurers = additionalMvcConfigurers;
	}

	/// <inheritdoc />
	public void Configure()
	{
		// Add MVC-adjacent services
		_builder.Services
			.AddEndpointsApiExplorer()
			.AddSwaggerGen()
			.AddScoped<INotifier, DefaultNotifier>();

		// Add and configure MVC
		var mvcbuilder = _builder.Services.AddMvc(o =>
		{
			foreach (var configurer in _mvcConfigurers)
			{
				configurer.Configure(o);
			}
		});

		foreach (var configurer in _additionalMvcConfigurers)
		{
			configurer.Configure(mvcbuilder);
		}

		_middlewareProvider.AddWithPriority(
			Priority.High,
			app =>
			{
				if (app.Environment.IsDevelopment())
				{
					app
						.UseSwagger()
						.UseSwaggerUI();
				}
			});

		_middlewareProvider.AddWithPriority(
			Priority.Lowest,
			app =>
			{
				app.MapControllers();
				app.MapRazorPages();
			});
	}
}
