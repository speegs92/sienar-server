namespace Sienar.Layouts;

public partial class NavMenu : IBrowserViewportObserver, IAsyncDisposable
{
	private List<List<MenuLink>> _menus = [];
	private bool _menuOpen;
	private ComponentDictionary _components = null;

	private bool Open
	{
		get => _breakpoint >= Breakpoint.Lg || _menuOpen;
		set => _menuOpen = value;
	}

	private DrawerVariant DrawerVariant => _breakpoint >= Breakpoint.Lg
		? DrawerVariant.Persistent
		: DrawerVariant.Temporary;

	/// <summary>
	/// The current browser breakpoint size
	/// </summary>
	private Breakpoint _breakpoint = Breakpoint.Lg;

	/// <inheritdoc />
	Guid IBrowserViewportObserver.Id { get; } = Guid.NewGuid();

	/// <inheritdoc />
	ResizeOptions? IBrowserViewportObserver.ResizeOptions { get; } = new()
	{
		ReportRate = 250,
		NotifyOnBreakpointOnly = true
	};

	/// <summary>
	/// The type of the layout
	/// </summary>
	[Parameter]
	public required Type LayoutType { get; set; } = null!;

	/// <summary>
	/// the menu names the nav menu should render
	/// </summary>
	[Parameter]
	public required IEnumerable<string> MenuNames { get; set; }

	[Inject]
	private IBrowserViewportService BrowserViewportService { get; set; } = null!;

	[Inject]
	private ComponentProvider ComponentProvider { get; set; } = null!;

	[Inject]
	private IMenuGenerator MenuGenerator { get; set; } = null!;

	[Inject]
	private AuthenticationStateProvider AuthState { get; set; } = null!;

	[Inject]
	private NavigationManager NavManager { get; set; } = null!;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		_components = ComponentProvider.Access(LayoutType);

		NavManager.LocationChanged += OnNavigate;
		AuthState.AuthenticationStateChanged += UpdateMenuAndRender;
		UpdateMenuAndRender(AuthState.GetAuthenticationStateAsync());
	}

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await BrowserViewportService.SubscribeAsync(this, fireImmediately: true);
		}
	}

	/// <inheritdoc />
	Task IBrowserViewportObserver.NotifyBrowserViewportChangeAsync(
		BrowserViewportEventArgs browserViewportEventArgs)
	{
		_breakpoint = browserViewportEventArgs.Breakpoint;
		return InvokeAsync(StateHasChanged);
	}

	/// <summary>
	/// Toggles the open state of the navigation drawer
	/// </summary>
	protected void ToggleDrawer() => _menuOpen = !_menuOpen;

	private async void UpdateMenuAndRender(Task<AuthenticationState> s)
	{
		await s;
		_menus.Clear();
		foreach (var name in MenuNames)
		{
			_menus.Add(await MenuGenerator.Create(name));
		}
		StateHasChanged();
	}

	private void OnNavigate(object? sender, LocationChangedEventArgs e)
	{
		if (!_menuOpen) return;
		_menuOpen = false;
		StateHasChanged();
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		await BrowserViewportService.UnsubscribeAsync(this);
		NavManager.LocationChanged -= OnNavigate;
		AuthState.AuthenticationStateChanged -= UpdateMenuAndRender;
		GC.SuppressFinalize(this);
	}
}
