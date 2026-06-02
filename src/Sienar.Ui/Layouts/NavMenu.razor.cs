using System.Reflection;

namespace Sienar.Layouts;

public partial class NavMenu : IDisposable
{
	private AuthenticationState? _lastAuthState;
	private Type? _lastPageType;

	private List<List<MenuLink>> _menus = [];
	private bool _menuOpen;
	private ComponentDictionary _components = null!;

	/// <summary>
	/// The type of the layout
	/// </summary>
	[CascadingParameter(Name = SienarFound.CascadingLayoutName)]
	public Type LayoutType { get; set; } = null!;

	[CascadingParameter]
	private RouteData RouteData { get; set; } = null!;

	[CascadingParameter]
	private Task<AuthenticationState>? AuthState { get; set; }

	[Inject]
	private ComponentProvider ComponentProvider { get; set; } = null!;

	[Inject]
	private GlobalComponentProvider GlobalComponentProvider { get; set; } = null!;

	[Inject]
	private IMenuGenerator MenuGenerator { get; set; } = null!;

	[Inject]
	private NavigationManager NavManager { get; set; } = null!;

	/// <inheritdoc />
	protected override async Task OnInitializedAsync()
	{
		_components = ComponentProvider.Access(LayoutType);

		NavManager.LocationChanged += OnNavigate;
		await UpdateMenuAndRender();
	}

	/// <inheritdoc />
	protected override async Task OnParametersSetAsync()
	{
		AuthenticationState? authState = null;
		if (AuthState is not null)
		{
			authState = await AuthState;
		}

		if (_lastAuthState != authState ||
			_lastPageType != RouteData.PageType)
		{
			_lastAuthState = authState;
			_lastPageType = RouteData.PageType;

			await UpdateMenuAndRender();
		}
	}

	/// <summary>
	/// Toggles the open state of the navigation drawer
	/// </summary>
	protected void ToggleDrawer() => _menuOpen = !_menuOpen;

	private async Task UpdateMenuAndRender()
	{
		_menus.Clear();
		var pageType = RouteData.PageType;
		var menuNames = pageType.GetCustomAttribute<MenusAttribute>()
			?.Names ?? GlobalComponentProvider.DefaultMenus;

		foreach (var name in menuNames)
		{
			_menus.Add(await MenuGenerator.Create(name));
		}

		// StateHasChanged();
	}

	private void OnNavigate(object? sender, LocationChangedEventArgs e)
	{
		if (!_menuOpen)
		{
			return;
		}

		_menuOpen = false;
		StateHasChanged();
	}

	/// <inheritdoc />
	public void Dispose()
	{
		NavManager.LocationChanged -= OnNavigate;
		GC.SuppressFinalize(this);
	}
}
