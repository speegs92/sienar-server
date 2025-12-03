using Sienar.Data;

namespace Sienar.Infrastructure;

/// <summary>
/// Handles notifications based on different types of <see cref="OperationResult{TResult}"/>
/// </summary>
public interface IOperationResultNotifier
{
	/// <summary>
	/// Unwraps a <see cref="WebResult{TResult}"/> and handles its notifications
	/// </summary>
	/// <param name="webResult">The web result, wrapped in an operation result</param>
	/// <typeparam name="T">The type of the result</typeparam>
	/// <returns>The operation result</returns>
	OperationResult<T> HandleWebResult<T>(OperationResult<WebResult<T>> webResult);

	/// <summary>
	/// Handles calling <see cref="INotifier"/> for an <see cref="OperationResult{TResult}"/>
	/// </summary>
	/// <param name="operationResult">The operation result</param>
	/// <typeparam name="T">The type of the result</typeparam>
	/// <returns>The operation result</returns>
	OperationResult<T> HandleOperationResult<T>(OperationResult<T> operationResult);
}
