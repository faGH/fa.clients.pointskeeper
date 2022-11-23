using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FrostAura.Clients.PointsKeeper.Data.Factories.DesignTime
{
  /// <summary>
  /// DB context factory for running migrations in design time.
  /// This allows for running migrations in the .Data project independently.
  /// </summary>
  public class PointsKeeperDbContextDesignTimeFactory : IDesignTimeDbContextFactory<PointsKeeperDbContext>
  {
    /// <summary>
    /// Factory method for producing the design time db context
    /// </summary>
    /// <param name="args"></param>
    /// <returns>Database context.</returns>
    public PointsKeeperDbContext CreateDbContext(string[] args)
    {
      var configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.Migrations.json")
          .Build();
      var builder = new DbContextOptionsBuilder<PointsKeeperDbContext>();
      var connectionString = configuration
          .GetConnectionString("PointsKeeperDbConnection");

      builder.UseSqlite(connectionString);

      Console.WriteLine($"Used connection string for configuration db: {connectionString}");

      return new PointsKeeperDbContext(builder.Options);
    }
  }
}
