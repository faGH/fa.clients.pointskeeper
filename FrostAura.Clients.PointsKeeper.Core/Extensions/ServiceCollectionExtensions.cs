using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FrostAura.Clients.PointsKeeper.Core.Extensions
{
  /// <summary>
  /// Extensions for IServiceCollection.
  /// </summary>
  public static class ServiceCollectionExtensions
  {
    /// <summary>
    /// Add all required application engine and manager services and config to the DI container.
    /// </summary>
    /// <param name="services">Application services collection.</param>
    /// <param name="config">Configuration for the application.</param>
    /// <returns>Application services collection.</returns>
    public static IServiceCollection AddFrostAuraCore(this IServiceCollection services)
    {
      return services;
    }
  }
}
