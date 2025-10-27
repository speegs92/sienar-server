using System.Collections.Generic;

namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="IDictionary{TKey,TValue}"/> extension methods for the <c>Sienar.Utils</c> assembly
/// </summary>
public static class DictionaryExtensions
{
	/// <summary>
	/// Tries to get the specified key value from a dictionary, defaulting to <c>null</c> if the value doesn't exist
	/// </summary>
	/// <param name="self">The dictionary to try to get the value from</param>
	/// <param name="key">The key for which to attempt to retrieve a value</param>
	/// <typeparam name="TKey">The type of the key</typeparam>
	/// <typeparam name="TValue">The type of the value</typeparam>
	/// <returns>The value if it exists, else <c>null</c></returns>
	public static TValue? TryGetValue<TKey, TValue>(
		this IDictionary<TKey, TValue> self,
		TKey key)
	{
		if (self.TryGetValue(key, out TValue? value))
		{
			return value;
		}

		return default;
	}
}
