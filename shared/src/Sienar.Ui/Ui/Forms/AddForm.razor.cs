using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sienar.Data;
using Sienar.Extensions;
using Sienar.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

/// <summary>
/// Automates adding a new entity
/// </summary>
/// <remarks>
/// The <see cref="BaseForm{T}.OnSubmit"/> parameter has no effect on <c>AddForm</c> because <c>AddForm</c> has its own submit logic.
/// </remarks>
/// <typeparam name="T">The type of the add DTO</typeparam>
public partial class AddForm<T>
{
	private static string EntityName => typeof(T)
		.GetEntityName();

	/// <summary>
	/// A function to execute if the form is submitted successfully
	/// </summary>
	/// <remarks>
	/// The arguments of the provided delegate will be resolved from the DI container. The only exception to this is the <c>OperationResult&lt;int?&gt;</c> returned by <see cref="IEntityCreateActor{T}.Create">IEntityCreateActor&lt;T&gt;.Create()</see>, which can be provided at any position (but is not required).
	/// </remarks>
	[Parameter]
	public Delegate? OnSuccess { get; set; }

	private Task HandleAdd(IEntityCreateActor<T> creator)
	{
		return SubmitRequest(async () =>
		{
			var result = await creator.Create(Value);

			if (result.Status is OperationStatus.Success)
			{
				await DelegateHandler.Handle(OnSuccess, result);
			}
		});
	}
}

