using FrostAura.Clients.PointsKeeper.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FrostAura.Clients.PointsKeeper.Data
{
  /// <summary>
  /// PointsKeeper database context.
  /// </summary>
  public class PointsKeeperDbContext : DbContext
  {
    /// <summary>
    /// Construct and allow for passing options.
    /// </summary>
    /// <param name="options">Db creation options.</param>
    public PointsKeeperDbContext(DbContextOptions<PointsKeeperDbContext> options)
        : base(options)
    { }

    public virtual DbSet<Team> Teams { get; set; }
    public virtual DbSet<Player> Players { get; set; }
    public virtual DbSet<Point> Points { get; set; }
  }
}
