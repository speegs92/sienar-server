namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="IServiceCollection"/> extension methods for the <c>Sienar.Utils.Blazor</c> assembly
/// </summary>
public static class SienarUtilsBlazorServiceCollectionExtensions
{
	/// <summary>
	/// Adds Sienar Blazor utilities to the DI contaziner
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddSienarBlazorUtilities(this IServiceCollection self)
	{
		self.TryAddScoped<SienarAuthenticationStateProvider>();
		self.TryAddScoped<AuthenticationStateProvider>(
			sp => sp.GetRequiredService<SienarAuthenticationStateProvider>());
		self.TryAddScoped<IDelegateHandler, DefaultDelegateHandler>();
		self.TryAddScoped<IScheduler, DefaultScheduler>();

		return self;
	}
}
