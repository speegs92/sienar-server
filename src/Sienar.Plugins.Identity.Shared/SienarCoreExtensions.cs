using System.Reflection;

namespace Sienar;

public static class SienarCoreExtensions
{
	public static string? GetMediaDirectory(this Enum self)
	{
		var a = self
			.GetType()
			.GetField(self.ToString())
			?.GetCustomAttribute<MediaDirectoryAttribute>();
		return a?.Directory;
	}
}