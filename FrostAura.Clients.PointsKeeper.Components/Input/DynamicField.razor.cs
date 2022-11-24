using FrostAura.Clients.PointsKeeper.Components.Abstractions;
using FrostAura.Clients.PointsKeeper.Shared.Attributes.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace FrostAura.Clients.PointsKeeper.Components.Input
{
  /// <summary>
  /// Dynamic field to render a form element for given property infomation.
  /// </summary>
  public partial class DynamicField : BaseComponent<DynamicField>
  {
    /// <summary>
    /// Get the instance. This instance will be used to fill out the values inputted by the user.
    /// </summary>
    [CascadingParameter]
    public EditContext CascadedEditContext { get; set; }
    /// <summary>
    /// The context of the model that this dynamic field represents.
    /// When this dynamic field component is at the root of an object, this value should be the EditContext model otherwise the nested model at hand.
    /// </summary>
    [Parameter]
    public object Model { get; set; }
    /// <summary>
    /// Property information to render an element for.
    /// </summary>
    [Parameter]
    public PropertyInfo PropertyInformation { get; set; }
    /// <summary>
    /// Whether to render a per-element validator.
    /// </summary>
    [Parameter]
    public bool ShouldRenderValidator { get; set; }
    /// <summary>
    /// Getter for the field's description.
    /// </summary>
    private string _fieldLabel
    {
      get
      {
        var descriptionAttribute = PropertyInformation
            .GetCustomAttribute<DescriptionAttribute>();

        return descriptionAttribute?.Description ?? PropertyInformation.Name;
      }
    }
    /// <summary>
    /// A unique field id for this particular fully qualified property name.
    /// </summary>
    private string _fieldId => $"{Model.GetType()}.{PropertyInformation.Name}";
    /// <summary>
    /// Collection of types that support being rendered by the dynamic field system together with which component to render for the type.
    /// </summary>
    private static readonly Dictionary<Type, Type> _typeToControlRendererMappings = new Dictionary<Type, Type>();

    protected override void OnInitialized()
    {
      Model = Model ?? CascadedEditContext.Model;

      // Register default form control mappings.
      RegisterRendererTypeControl<string, InputText>();
      RegisterRendererTypeControl<int, InputNumber<int>>();
      RegisterRendererTypeControl<DateTime, InputDate<DateTime>>();
      RegisterRendererTypeControl<bool, InputCheckbox>();

      base.OnInitialized();
    }

    /// <summary>
    /// Register a new control to be rendered when a given field type is encountered. The control type has to be derived from InputBase<typeparamref name="TFieldType"/>.
    /// </summary>
    /// <typeparam name="TFieldType">Fielt type to map the control for.</typeparam>
    /// <typeparam name="TControlToRenderType">Control type to render for the field.</typeparam>
    public static void RegisterRendererTypeControl<TFieldType, TControlToRenderType>() where TControlToRenderType : InputBase<TFieldType>
    {
      _typeToControlRendererMappings[typeof(TFieldType)] = typeof(TControlToRenderType);
    }

    /// <summary>
    /// Generate a renderable input component section by determining the property type and initiating the generic method to perform the field's generation.
    /// </summary>
    /// <returns>Renderable component.</returns>
    private RenderFragment GenerateInputComponent()
    {
      var method = typeof(DynamicField)
          .GetMethod(nameof(DynamicField.GenerateRenderTreeForInputField), BindingFlags.NonPublic | BindingFlags.Instance);
      var appendInputComponentToRenderer = method
          .MakeGenericMethod(PropertyInformation.PropertyType);

      return builder =>
      {
        appendInputComponentToRenderer.Invoke(this, new object[] { builder });
      };
    }

    /// <summary>
    /// Generate a validation component to back the field.
    /// </summary>
    /// <returns>Renderable component.</returns>
    private RenderFragment GenerateValidationComponent()
    {
      var dynamicValidationMessageType = typeof(DynamicValidationMessage<object>)
          .GetGenericTypeDefinition()
          .MakeGenericType(PropertyInformation?.PropertyType);

      if (!ShouldRenderValidator) return builder => { };

      return builder =>
      {
        builder.OpenComponent(0, dynamicValidationMessageType);
        builder.AddAttribute(1, nameof(DynamicValidationMessage<object>.PropertyInformation), PropertyInformation);
        builder.AddAttribute(2, nameof(DynamicValidationMessage<object>.Model), Model);
        builder.CloseComponent();
      };
    }

    /// <summary>
    /// Generate a randerable section for the given property information.
    /// </summary>
    /// <typeparam name="TValue">Property value type.</typeparam>
    /// <param name="builder">Render tree.</param>
    private void GenerateRenderTreeForInputField<TValue>(RenderTreeBuilder builder)
    {
      var componentType = GetComponentTypeToRenderForValueType<TValue>();

      if (componentType == default)
      {
        GenerateRenderTreeForChildInputFields<TValue>(builder);

        return;
      }

      var constant = Expression.Constant(Model, Model.GetType());
      var exp = Expression.Property(constant, PropertyInformation.Name);
      var lambda = Expression.Lambda(exp);
      var castedLambda = (Expression<Func<TValue>>)lambda;
      var currentValue = (TValue)PropertyInformation.GetValue(Model);

      builder.OpenComponent(0, componentType);
      builder.AddAttribute(1, "id", _fieldId);
      // The following is a replacement for the bind-value property.
      builder.AddAttribute(1, nameof(InputBase<TValue>.Value), currentValue);
      builder.AddAttribute(2, nameof(InputBase<TValue>.ValueExpression), castedLambda);
      builder.AddAttribute(3, nameof(InputBase<TValue>.ValueChanged), RuntimeHelpers.TypeCheck(
          EventCallback.Factory.Create(
              this,
              EventCallback.Factory.CreateInferred(this, val => PropertyInformation.SetValue(Model, val),
              (TValue)PropertyInformation.GetValue(Model)))));
      builder.CloseComponent();
    }

    /// <summary>
    /// Generate a randerable section for the given property information's children properties.
    /// </summary>
    /// <typeparam name="TValue">Property value type.</typeparam>
    /// <param name="builder">Render tree.</param>
    private void GenerateRenderTreeForChildInputFields<TValue>(RenderTreeBuilder builder)
    {
      var type = typeof(TValue);
      var properties = type
          .GetProperties()
          .Where(p => p.GetCustomAttribute<FieldIgnoreAttribute>() == default)
          .Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() == default)
          .Where(p => p.GetCustomAttribute<DatabaseGeneratedAttribute>() == default)
          .Where(p => !p.GetAccessors().First().IsVirtual)
          .ToArray();
      var nestedModel = Model
        .GetType()
        .GetProperties()
        .First(p => p.PropertyType == type)
        .GetValue(Model);

      for (int i = 0; i < properties.Length; i++)
      {
        var property = properties[i];
        builder.OpenComponent(0, typeof(DynamicField));
        builder.AddAttribute(1, nameof(EnableDemoMode), EnableDemoMode);
        builder.AddAttribute(2, nameof(PropertyInformation), property);
        builder.AddAttribute(3, nameof(Model), nestedModel);
        builder.CloseComponent();
      }
    }

    /// <summary>
    /// Determine which object type to render for a given data type.
    /// </summary>
    /// <typeparam name="TValue">Data type.</typeparam>
    /// <returns>Object type to render.</returns>
    private Type GetComponentTypeToRenderForValueType<TValue>()
    {
      if (!_typeToControlRendererMappings.ContainsKey(typeof(TValue)))
      {
        Logger.LogWarning($"The type '{typeof(TValue).FullName}' is not supported by DynamicField. Call FrostAura.Standard.Components.Razor.Input.DynamicField.RegisterRendererTypeControl in order to map a renderer.");

        return default;
      }

      return _typeToControlRendererMappings[typeof(TValue)];
    }
  }
}
