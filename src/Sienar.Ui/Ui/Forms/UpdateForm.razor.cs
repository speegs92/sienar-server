using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

/// <summary>
/// Automates updating an existing entity
/// </summary>
/// <typeparam name="TViewDto">The type of the view DTO</typeparam>
/// <typeparam name="TEditDto">The type of the edit DTO</typeparam>
public partial class UpdateForm<TViewDto, TEditDto>
{
	private string _displayName = typeof(TViewDto).GetEntityName();

	/// <summary>
	/// The ID of the editing entity
	/// </summary>
	[Parameter]
	public int EntityId { get; set; }

	/// <summary>
	/// A function to call when the view-DTO is loaded
	/// </summary>
	[Parameter]
	public Action<TViewDto>? OnLoaded { get; set; }

	/// <summary>
	/// A function to execute if the form is submitted successfully
	/// </summary>
	/// <remarks>
	/// The arguments of the provided delegate will be resolved from the DI container. The only exception to this is the <c>OperationResult&lt;bool?&gt;</c> returned by <see cref="IEntityUpdateActor{T}.Update">IEntityUpdateActor&lt;T&gt;.Update()</see>, which can be provided at any position (but is not required).
	/// </remarks>
	[Parameter]
	public Delegate? OnSuccess { get; set; }

	[Inject]
	private IEntityReadActor<TViewDto> Reader { get; set; } = null!;

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

			OnLoaded?.Invoke(result.Result);
			Mapper.Map(result.Result, Value);

			GenerateDisplayName(result.Result);
		});

	private Task HandleUpdate(IEntityUpdateActor<TEditDto> updater)
	{
		return SubmitRequest(async () =>
		{
			var result = await updater.Update(Value);

			if (result.Status is OperationStatus.Success)
			{
				await DelegateHandler.Handle(OnSuccess, result);
			}
		});
	}

	private void GenerateDisplayName(TViewDto viewDto)
	{
		var dtoType = typeof(TViewDto);
		var propName = dtoType.GetCustomAttribute<DisplayPropertyAttribute>()?.Property;

		if (!string.IsNullOrWhiteSpace(propName))
		{
			var newName = dtoType
				.GetProperty(propName)
				?.GetValue(viewDto)
				?.ToString();

			if (!string.IsNullOrEmpty(newName))
			{
				_displayName = newName;
			}
		}
	}
}
