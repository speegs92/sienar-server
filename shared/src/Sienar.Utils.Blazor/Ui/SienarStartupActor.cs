namespace Sienar.Ui;

/// <summary>
/// A component used to call startup initialization tasks after the initial render
/// </summary>
public class SienarStartupActor : ComponentBase
{
	[Inject]
	private IStatusActor<Startup> StartupActor { get; set; } = null!;

	/// <inheritdoc />
	protected override async Task OnInitializedAsync()
	{
		await StartupActor.Execute(new Startup());
	}
}
