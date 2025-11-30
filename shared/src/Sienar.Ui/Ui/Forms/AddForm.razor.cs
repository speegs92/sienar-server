using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sienar.Data;
using Sienar.Extensions;
using Sienar.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

public partial class AddForm<TAddDto>
{
	private static string EntityName => typeof(TAddDto)
		.GetEntityName();

	/// <summary>
	/// A function to execute if the form is submitted successfully
	/// </summary>
	/// <remarks>
	/// The arguments of the provided delegate will be resolved from the DI container. The only exception to this is the <c>OperationResult&lt;int?&gt;</c> returned by <see cref="IEntityWriter{TEntity}.Create">IEntityWriter&lt;TEntity&gt;.Create()</see>, which can be provided at any position (but is not required).
	/// </remarks>
	[Parameter]
	public Delegate? OnSuccess { get; set; }

	[Inject]
	private IEntityWriter<TAddDto> Writer { get; set; } = null!;

	private Task Submit()
	{
		return SubmitRequest(async () =>
		{
			var result = await Writer.Create(Model);

			if (result.Status is OperationStatus.Success)
			{
				DelegateHandler.Handle(OnSuccess, result);
			}
		});
	}
}

