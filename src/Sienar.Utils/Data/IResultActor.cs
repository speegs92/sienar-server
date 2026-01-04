namespace Sienar.Data;

/// <summary>
/// A service that accepts no input and returns output
/// </summary>
/// <typeparam name="TResult">the type of the output</typeparam>
public interface IResultActor<TResult> where TResult : IResult
{
	/// <summary>
	/// Executes the request
	/// </summary>
	/// <returns>the output of the operation, or <c>null</c></returns>
	Task<OperationResult<TResult>> Execute();
}