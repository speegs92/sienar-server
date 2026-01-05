using Microsoft.Extensions.DependencyInjection;
using Sienar.Configuration;
using TestProject.Data;

namespace TestProject;

public class TestProjectMvcBuilderConfigurer : IConfigurer<IMvcBuilder>
{
	public void Configure(IMvcBuilder builder)
	{
		builder.ConfigureApplicationPartManager(o =>
		{
			o.FeatureProviders.Add(new SienarIdentityControllerFeatureProvider<AppUser>());
		});
	}
}
