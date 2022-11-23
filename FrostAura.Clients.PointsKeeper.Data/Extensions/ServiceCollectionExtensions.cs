using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FrostAura.Clients.PointsKeeper.Data.Extensions
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
    public static IServiceCollection AddFrostAuraResources(this IServiceCollection services, IConfiguration config)
    {
      return services
          .AddConfig(config)
          .AddServices();
    }

    /// <summary>
    /// Add all required application engines configuration to the DI container.
    /// </summary>
    /// <param name="services">Application services collection.</param>
    /// <param name="config">Configuration for the application.</param>
    /// <returns>Application services collection.</returns>
    private static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration config)
    {
      var connectionString = config.GetConnectionString("PointsKeeperDbConnection");
      var migrationsAssembly = typeof(PointsKeeperDbContext).GetTypeInfo().Assembly.GetName().Name;

      return services
          .AddDbContext<PointsKeeperDbContext>(config =>
          {
            config.UseSqlite(connectionString);
          });
    }

    /// <summary>
    /// Add all required application engine services to the DI container.
    /// </summary>
    /// <param name="services">Application services collection.</param>
    /// <returns>Application services collection.</returns>
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
      // Register scoped services here.

      return services;
    }
  }
}
