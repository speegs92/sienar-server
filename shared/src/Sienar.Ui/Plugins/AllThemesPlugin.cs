using Sienar.Html;

namespace Sienar.Plugins;

/// <summary>
/// Configures the Sienar app to support Sienar's UI framework with all themes available
/// </summary>
/// <remarks>
/// It isn't recommended to use this plugin in production, as it includes <b>every</b> theme. This is mostly useful for development when comparing multiple themes.
/// </remarks>
public class AllThemesPlugin : IPlugin
{
	private readonly StyleProvider _styleProvider;
	private readonly ScriptProvider _scriptProvider;

	public AllThemesPlugin(
		StyleProvider styleProvider,
		ScriptProvider scriptProvider)
	{
		_styleProvider = styleProvider;
		_scriptProvider = scriptProvider;
	}

	/// <inheritdoc />
	public void Configure()
	{
		ConfigureStyles();
		ConfigureScripts();
	}

	private void ConfigureStyles()
	{
		_styleProvider.AddRange(
			[
				"/_content/Sienar.Ui/sienar-utils.css",
				"/_content/Sienar.Ui/theme-sienar.css"
			]);
	}

	private void ConfigureScripts()
	{
		_scriptProvider.Add(new ScriptResource
		{
			Src = "/_content/Sienar.Ui/sienar-ui.js",
			ShouldDefer = true
		});
	}
}
