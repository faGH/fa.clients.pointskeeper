using FrostAura.Clients.PointsKeeper.Components.Abstractions;
using FrostAura.Clients.PointsKeeper.Components.Enums.DynamicForm;
using FrostAura.Clients.PointsKeeper.Components.Models;
using FrostAura.Clients.PointsKeeper.Shared.Attributes.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace FrostAura.Clients.PointsKeeper.Components.Input
{
  /// <summary>
  /// Component for generating an input form automatically based on a given type and instance of the type.
  /// </summary>
  /// <typeparam name="TDataContextType">Data context model type.</typeparam>
  public partial class DynamicForm<TDataContextType> : BaseComponent<DynamicForm<TDataContextType>> where TDataContextType : new()
  {
    /// <summary>
    /// Form data context to use when updating / editing.
    /// </summary>
    [Parameter]
    public TDataContextType DataContext { get; set; } = new TDataContextType();
    /// <summary>
    /// Callback for when the form is submitted with valid values.
    /// </summary>
    [Parameter]
    public EventCallback<TDataContextType> OnValidSubmit { get; set; }
    /// <summary>
    /// Submit button text to show.
    /// </summary>
    [Parameter]
    public string SubmitButtonText { get; set; } = "Submit";
    /// <summary>
    /// Where to show a full validation summary,
    /// </summary>
    [Parameter]
    public ValidationSummaryPosition ValidationSummaryPosition { get; set; }
    /// <summary>
    /// Collection of form property effects to apply.
    /// </summary>
    [Parameter]
    public List<FormPropertyEffect> PropertyEffects { get; set; } = new List<FormPropertyEffect>();
    /// <summary>
    /// Get data context property information.
    /// </summary>
    private IEnumerable<PropertyInfo> _dataContextProperties => DataContext?
        .GetType()
        .GetProperties()
        .Where(p => p.GetCustomAttribute<FieldIgnoreAttribute>() == default)
        .Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() == default)
        .Where(p => p.GetCustomAttribute<DatabaseGeneratedAttribute>() == default)
        .Where(p => !p.GetAccessors().First().IsVirtual)
        .ToArray();

    /// <summary>
    /// Handler for when the form has been successfully submitted.
    /// </summary>
    /// <param name="context">Edit context.</param>
    public void HandleOnValidSubmit(EditContext context)
    {
      if (!OnValidSubmit.HasDelegate) return;

      OnValidSubmit.InvokeAsync((TDataContextType)context.Model);
      StateHasChanged();
    }
  }
}
