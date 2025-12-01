using System;
using System.Threading.Tasks;
using Sienar.Infrastructure;
using Sienar.Security;

namespace Sienar.Pages;

/// <summary>
/// A component that provides time tracking and form resetting
/// </summary>
/// <typeparam name="T">The type of the form model</typeparam>
public class FormPage<T> : ActionPage
	where T : new()
{
	/// <summary>
	/// The model used in requests sent from the page
	/// </summary>
	protected T Model = new();

	/// <summary>
	/// The time when the form was started. Used for bot tracking purposes
	/// </summary>
	protected DateTime FormStartedTime { get; private set; } = DateTime.Now;

	/// <summary>
	/// Resets the form
	/// </summary>
	protected void Reset()
	{
		Model = new T();
		FormStartedTime = DateTime.Now;
	}

	/// <inheritdoc />
	protected override Task<TResult?> SubmitRequest<TResult>(Func<Task<OperationResult<TResult>>> action)
		where TResult : default
	{
		if (Model is Honeypot honeypot)
		{
			honeypot.TimeToComplete = DateTime.Now - FormStartedTime;
		}

		return base.SubmitRequest(action);
	}
}
