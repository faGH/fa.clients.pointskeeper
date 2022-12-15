using FrostAura.Libraries.Data.Models.EntityFramework;
using System.ComponentModel;
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
  public class Player : BaseNamedEntity
  {
    /// <summary>
    /// Team's logo content.
    /// </summary>
    [Description("Player Logo")]
    public string Logo { get; set; } = "https://via.placeholder.com/256x256";
    /// <summary>
    /// The unique id of the team the respective player belongs to.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "A valid team is required.")]
    public int TeamId { get; set; }
    /// <summary>
    /// The team context that the respective player belongs to.
    /// </summary>
    [ForeignKey(nameof(TeamId))]
    public virtual Team Team { get; set; }
    /// <summary>
    /// Collection of points the respective player has aquired.
    /// </summary>
    [NotMapped]
    public virtual ICollection<Point> Points { get; set; } = new List<Point>();
  }
}
