using System.Net.Http;

namespace Sienar.Data;

/// <summary>
/// Describes a RESTful endpoint for a DTO
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class RestEndpointAttribute : Attribute
{
	/// <summary>
	/// The REST endpoint
	/// </summary>
	public string Endpoint { get; }

	/// <summary>
	/// The HTTP method
	/// </summary>
	public HttpMethod? Method { get; }

	/// <summary>
	/// Creates a new instance of <c>RestEndpointAttribute</c>
	/// </summary>
	/// <param name="endpoint">The endpoint at which the DTO should interact</param>
	/// <param name="method">The HTTP method with which the DTO should interact</param>
	public RestEndpointAttribute(
		string endpoint,
		string? method = null)
	{
		Endpoint = endpoint;
		Method = MapToHttpMethod(method);
	}

	private static HttpMethod? MapToHttpMethod(string? method)
	{
		if (string.IsNullOrEmpty(method))
		{
			return null;
		}

		return method switch
		{
			HttpMethods.Connect => HttpMethod.Connect,
			HttpMethods.Delete => HttpMethod.Delete,
			HttpMethods.Get => HttpMethod.Get,
			HttpMethods.Head => HttpMethod.Head,
			HttpMethods.Options => HttpMethod.Options,
			HttpMethods.Patch => HttpMethod.Patch,
			HttpMethods.Post => HttpMethod.Post,
			HttpMethods.Put => HttpMethod.Put,
			HttpMethods.Trace => HttpMethod.Trace,
			_ => throw new ArgumentException(
				"The provided HTTP method name must be one of the valid HTTP verbs",
				nameof(method))
		};
	}
}
