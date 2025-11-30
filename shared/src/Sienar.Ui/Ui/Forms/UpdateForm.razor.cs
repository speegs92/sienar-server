using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sienar.Data;
using Sienar.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

public partial class UpdateForm<TViewDto, TEditDto>
{
	/// <summary>
	/// The ID of the editing entity
	/// </summary>
	[Parameter]
	public int EntityId { get; set; }

	/// <summary>
	/// A function to retrieve the name of the editing entity
	/// </summary>
	[Parameter]
	public required Func<TEditDto, string> For { get; set; }

	/// <summary>
	/// A function to call when the view-DTO is loaded
	/// </summary>
	[Parameter]
	public EventCallback<TViewDto> OnLoaded { get; set; }

	/// <summary>
	/// A function to execute if the form is submitted successfully
	/// </summary>
	/// <remarks>
	/// The arguments of the provided delegate will be resolved from the DI container. The only exception to this is the <c>OperationResult&lt;bool?&gt;</c> returned by <see cref="IEntityWriter{TEntity}.Update">IEntityWriter&lt;TEntity&gt;.Update()</see>, which can be provided at any position (but is not required).
	/// </remarks>
	[Parameter]
	public Delegate? OnSuccess { get; set; }

	[Inject]
	private IEntityReader<TViewDto> Reader { get; set; } = null!;

	[Inject]
	private IEntityWriter<TEditDto> Writer { get; set; } = null!;

	[Inject]
	private IMapper<TViewDto, TEditDto> Mapper { get; set; } = null!;

	/// <inheritdoc />
	protected override Task OnInitializedAsync()
		=> SubmitRequest(async () =>
		{
			var result = await Reader.Read(EntityId);

			if (result.Status is not OperationStatus.Success ||
				result.Result is null)
			{
				return;
			}

			await OnLoaded.InvokeAsync(result.Result);
			Mapper.Map(result.Result, Model);
		});

	private Task Submit()
	{
		return SubmitRequest(async () =>
		{
			var result = await Writer.Update(Model);

			if (result.Status is OperationStatus.Success)
			{
				DelegateHandler.Handle(OnSuccess, result);
			}
		});
	}
}
