using FrostAura.Libraries.Data.Models.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace FrostAura.Clients.PointsKeeper.Shared.Models
{
  /// <summary>
  /// Player entity model.
  /// </summary>
  [Table("Players")]
  [DebuggerDisplay("Name: {Name}")]
  public class Player : BaseEntity
  {
    /// <summary>
    /// Entity name / short description.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = $"A valid {nameof(Player)} name is required.")]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The unique id of the team the respective player belongs to.
    /// </summary>
    [Required]
    public int TeamId { get; set; }
    /// <summary>
    /// The team context that the respective player belongs to.
    /// </summary>
    [ForeignKey(nameof(TeamId))]
    public virtual Team Team { get; set; }
    /// <summary>
    /// Collection of points the respective player has aquired.
    /// </summary>
    public virtual ICollection<Point> Points { get; set; } = new List<Point>();
  }
}
