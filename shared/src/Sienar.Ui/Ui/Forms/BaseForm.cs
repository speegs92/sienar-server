using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Sienar.Infrastructure;
using Sienar.Pages;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

/// <summary>
/// The base form class
/// </summary>
/// <typeparam name="T">The type of the form's data model</typeparam>
public class BaseForm<T> : ActionPage
	where T : new()
{
	private DateTime _formStartedTime = DateTime.Now;

	/// <summary>
	/// The model used in requests
	/// </summary>
	[Parameter]
	public T Value { get; set; } = new();

	/// <summary>
	/// The callback invoked when the model changes
	/// </summary>
	[Parameter]
	public EventCallback<T> ValueChanged { get; set; }

	/// <summary>
	/// The color of the form
	/// </summary>
	[Parameter]
	public Color ThemeColor { get; set; } = Color.Primary;

	/// <summary>
	/// A callback to call when the form should be reset
	/// </summary>
	[Parameter]
	public EventCallback<MouseEventArgs> OnReset { get; set; }

	/// <summary>
	/// Additional actions to add to the card's actions
	/// </summary>
	[Parameter]
	public RenderFragment? MoreActions { get; set; }

	/// <summary>
	/// Information the user needs to complete the form
	/// </summary>
	[Parameter]
	public RenderFragment? Information { get; set; }

	/// <summary>
	/// The display title for the form
	/// </summary>
	[Parameter]
	public string? Title { get; set; }

	/// <summary>
	/// The HTML tag with which to render the title
	/// </summary>
	[Parameter]
	public string? TitleTag { get; set; }

	/// <summary>
	/// The text to show in the submit button
	/// </summary>
	[Parameter]
	public string? SubmitText { get; set; }

	/// <summary>
	/// The text to show in the reset button. Defaults to <b>Reset</b>
	/// </summary>
	[Parameter]
	public string? ResetText { get; set; } = "Reset";

	/// <summary>
	/// The icon to display in the upper-right corner of the form
	/// </summary>
	[Parameter]
	public string? Icon { get; set; }

	/// <summary>
	/// The title of the icon to display in the upper-right corner of the form
	/// </summary>
	[Parameter]
	public string? IconTitle { get; set; }

	/// <summary>
	/// Whether to show the reset button
	/// </summary>
	[Parameter]
	public bool ShowReset { get; set; }

	/// <summary>
	/// Whether to hide the reset button
	/// </summary>
	[Parameter]
	public bool HideSubmit { get; set; }

	/// <summary>
	/// Whether the form should be rendered with square corners
	/// </summary>
	[Parameter]
	public bool Square { get; set; }

	/// <summary>
	/// A function to execute on submit
	/// </summary>
	/// <remarks>
	/// The arguments of the provided delegate will be resolved from the DI container. The only exception to this is the <c>TModel</c> from the underlying form, which can be provided at any position (but is not required).
	/// </remarks>
	[Parameter]
	public Delegate? OnSubmit { get; set; }

	/// <summary>
	/// The form fields to render
	/// </summary>
	[Parameter]
	public required RenderFragment<T> Fields { get; set; }

	/// <summary>
	/// The <see cref="IDelegateHandler"/>
	/// </summary>
	[Inject]
	protected IDelegateHandler DelegateHandler { get; set; } = null!;

	/// <summary>
	/// Handles form submissions
	/// </summary>
	protected Task HandleSubmit()
		=> SubmitRequest(() =>
		{
			if (Value is Sienar.Security.Honeypot honeypot)
			{
				honeypot.TimeToComplete = DateTime.Now - _formStartedTime;
			}

			return DelegateHandler.Handle(OnSubmit, Value);
		});

	/// <summary>
	/// Handles form reset events
	/// </summary>
	/// <param name="sender">The event sender</param>
	/// <param name="args">The mouse event</param>
	protected async Task HandleReset(object? sender, MouseEventArgs args)
	{
		Value = new T();
		_formStartedTime = DateTime.Now;

		await OnReset.InvokeAsync(args);
	}
}