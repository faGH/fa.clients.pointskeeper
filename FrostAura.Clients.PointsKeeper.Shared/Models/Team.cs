using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace FrostAura.Clients.PointsKeeper.Shared.Models
{
  /// <summary>
  /// Team entity model.
  /// </summary>
  [Table("Teams")]
  [DebuggerDisplay("Name: {Name}")]
  public class Team : BaseNamedEntity
  {
    /// <summary>
    /// Team's logo content.
    /// </summary>
    [Description("Team Logo")]
    public string Logo { get; set; }
    /// <summary>
    /// Collection of players under the respective team.
    /// </summary>
    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
  }
}
