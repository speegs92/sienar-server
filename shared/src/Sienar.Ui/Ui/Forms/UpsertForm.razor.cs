using System;
using Microsoft.AspNetCore.Components;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

/// <summary>
/// Automates upserting an entity
/// </summary>
/// <typeparam name="TViewDto">The type of the view DTO</typeparam>
/// <typeparam name="TUpsertDto">The type of the upsert DTO</typeparam>
public partial class UpsertForm<TViewDto, TUpsertDto>
{
	/// <summary>
	/// The ID of the entity being updated, if any
	/// </summary>
	[Parameter]
	public int? EntityId { get; set; }

	/// <summary>
	/// A function to call when the view-DTO is loaded
	/// </summary>
	[Parameter]
	public Action<TViewDto>? OnLoaded { get; set; }

	/// <summary>
	/// A function to execute if the add form is submitted successfully
	/// </summary>
	/// <remarks>
	/// The arguments of the provided delegate will be resolved from the DI container. The only exception to this is the <c>OperationResult&lt;int?&gt;</c> returned by <see cref="Sienar.Data.IEntityCreateActor{T}.Create">IEntityCreateActor&lt;T&gt;.Create()</see>, which can be provided at any position (but is not required).
	/// </remarks>
	[Parameter]
	public Delegate? OnAddSuccess { get; set; }

	/// <summary>
	/// A function to execute if the form is submitted successfully
	/// </summary>
	/// <remarks>
	/// The arguments of the provided delegate will be resolved from the DI container. The only exception to this is the <c>OperationResult&lt;bool?&gt;</c> returned by <see cref="Sienar.Data.IEntityUpdateActor{T}.Update">IEntityUpdateActor&lt;T&gt;.Update()</see>, which can be provided at any position (but is not required).
	/// </remarks>
	[Parameter]
	public Delegate? OnUpdateSuccess { get; set; }
}

