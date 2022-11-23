using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace FrostAura.Clients.PointsKeeper.Components.Abstractions
{
  /// <summary>
  /// FrostAura base component for core and shared functionality.
  /// </summary>
  public abstract class BaseComponent<TComponentType> : ComponentBase
  {
    /// <summary>
    /// Unique component instance identifier.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    /// <summary>
    /// Whether to enable demo defaults for this component.
    /// </summary>
    [Parameter]
    public bool EnableDemoMode { get; set; }
    /// <summary>
    /// Navigation manager.
    /// </summary>
    [Inject]
    protected NavigationManager NavigationManager { get; set; }
    /// <summary>
    /// Instance logger.
    /// </summary>
    [Inject]
    protected ILogger<TComponentType> Logger { get; set; }
  }
}
