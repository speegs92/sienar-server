using Microsoft.AspNetCore.Builder;

namespace Sienar.Plugins;

/// <summary>
/// Configures the Sienar app to support MudBlazor
/// </summary>
public class MudBlazorPlugin : IPlugin
{
	private readonly WebApplicationBuilder _builder;
	private readonly ScriptProvider _scriptProvider;
	private readonly StyleProvider _styleProvider;
	private readonly IConfigurer<MudServicesConfiguration>? _mudConfigurer;

	/// <summary>
	/// Creates a new instance of <c>MudBlazorPlugin</c>
	/// </summary>
	public MudBlazorPlugin(
		WebApplicationBuilder builder,
		ScriptProvider scriptProvider,
		StyleProvider styleProvider,
		IConfigurer<MudServicesConfiguration>? mudConfigurer = null)
	{
		_builder = builder;
		_scriptProvider = scriptProvider;
		_styleProvider = styleProvider;
		_mudConfigurer = mudConfigurer;
	}

	/// <inheritdoc />
	public void Configure()
	{
		_builder.Services.AddMudServices(
			o => _mudConfigurer?.Configure(o));
		
		_scriptProvider.Add(new ScriptResource
		{
			Src = "/_content/MudBlazor/MudBlazor.min.js",
			ShouldDefer = true
		});

		_styleProvider.Add("/_content/MudBlazor/MudBlazor.min.css");
	}
}
