using FrostAura.Libraries.Data.Models.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace FrostAura.Clients.PointsKeeper.Shared.Models
{
  /// <summary>
  /// Team entity model.
  /// </summary>
  [Table("Teams")]
  [DebuggerDisplay("Name: {Name}")]
  public class Team : BaseEntity
  {
    /// <summary>
    /// Entity name / short description.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = $"A valid {nameof(Team)} name is required.")]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Collection of players under the respective team.
    /// </summary>
    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
  }
}
