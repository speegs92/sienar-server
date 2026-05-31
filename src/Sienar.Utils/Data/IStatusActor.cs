namespace Sienar.Data;

// ReSharper disable once TypeParameterCanBeVariant
/// <summary>
/// A service that accepts input and returns a <c>bool</c> indicating the success status of the operation
/// </summary>
/// <typeparam name="TRequest">the type of the input</typeparam>
public interface IStatusActor<TRequest> where TRequest : IRequest
{
	/// <summary>
	/// Executes the request
	/// </summary>
	/// <param name="request">the input of the operation</param>
	/// <returns>whether the operation was successful</returns>
	Task<OperationResult<bool>> Execute(TRequest request);
}